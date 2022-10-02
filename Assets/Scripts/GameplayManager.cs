using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Time Management")]
    [SerializeField]
    private float timeMultiplier = 60f;

    private DateTime currentTime;

    [Header("Tuning")]
    public List<HourTuning> hours = new List<HourTuning>(24);

    public List<Person> allPeople = new List<Person>();

    private List<RuntimeSlide> allSlides = new List<RuntimeSlide>();

    public float totalWater = 1000;

    public float availableWater = 0;

    List<Person> tagRemoval = new List<Person>();

    public int staffAvailable = 12;

    public float avgHappieness = 0f;

    void Start()
    {
        currentTime = new DateTime(2022, 08, 01, 7, 45, 0);
        GameObject[] slides = GameObject.FindGameObjectsWithTag("WaterSlide");
        foreach(GameObject go in slides)
        {
            allSlides.Add(go.GetComponent<RuntimeSlide>());
        }
    }

    private void FixedUpdate()
    {
        CreateNewPeople();
        foreach (Person p in allPeople)
        {
            if(p.inLine)
            {
                p.timeInLine += 0.012f;
            }
        }
    }

    void Update()
    {
        UpdateTime();
        FlagTimedOut();
        setWaterUsage();
        closeRide();
        ProcessLineups();
    }

    public void CreateNewPeople()
    {
        if(GetMinute() % 2 == 0)
        {
            // every other "minute"
            return;
        }
        HourTuning ht = hours[GetHour()];
        for (int i = 0; i < ht.spawnRate; i++)
        {
            if (UnityEngine.Random.Range(0, 100) < 9)
            {
                allPeople.Add(new Person(UnityEngine.Random.Range(0, 100) < ht.adultProbability, UnityEngine.Random.Range(ht.minDurration, ht.maxDurration),
                    ht.preLunch, UnityEngine.Random.Range(1, 6), currentTime));
            }
        }
    }

    public int GetTotalPeople()
    {
        int counter = 0;
        foreach(Person p in allPeople) 
        {
            counter += p.partySize;
        }
        return counter;
    }
    
    private void FlagTimedOut()
    {
        foreach (Person current in allPeople)
        {
            TimeSpan length = currentTime.Subtract(current.startTime);
            if (length.TotalMinutes > current.totalTime)
            {
                tagRemoval.Add(current);
            }
        }
    }

    public void closeRide()
    {
        foreach(RuntimeSlide rs in allSlides)
        {
            if(rs.closingSoon && rs.queueCleared())
            {
                rs.closed = true;
                rs.closingSoon = false;
            }
        }
    }

    public void ProcessLineups()
    {
        //Debug.Log("Lineup processing");
        // Process each water slide removing people from the queues
        foreach(RuntimeSlide rs in allSlides)
        {
            if(rs.lineup.Count > 0)
            {
                rs.capacityThisTick += rs.getCapacity();
            }
            //Debug.Log($"Slide {rs.name}: Capacity this tick {rs.capacityThisTick}, lineup length {rs.lineup.Count}");

            while(rs.lineup.Count > 0 && rs.capacityThisTick > rs.lineup.Peek().partySize)
            {
                Person served = rs.lineup.Dequeue();
                served.inLine = false;
                rs.capacityThisTick -= served.partySize;
                rs.NotifyRidership(served.partySize);
                UpdateHappieness(served.timeInLine, served.previousDemand);
                if (!served.ridesRidden.Contains(rs))
                {
                    served.ridesRidden.Add(rs);
                }
            }
        }
        // if people are not in a queue, remove them
        for(int i = 0, count = tagRemoval.Count; i < count; i++)
        {
            if (!tagRemoval[i].inLine)
            {
                allPeople.Remove(tagRemoval[i]);
            }
        }
        tagRemoval.Clear();
        // for each person not in a queue, add them to one (this is the hard part)
        foreach (Person p in allPeople)
        {
            if(!p.inLine)
            {
                RuntimeSlide highestDemandSoFar = null;
                bool unriddenRide = false;

                float previousDemand = 0;
                for(int i = 0, count = allSlides.Count; i < count; i++)
                {
                    if(!allSlides[i].closingSoon && !allSlides[i].closed) // Closing soon check on ride
                    {
                        RuntimeSlide current = allSlides[i];
                        bool ridden = p.ridesRidden.Contains(current);
                        bool matchesAge = p.adults == current.parent.adultRide;
                        float foodImpact = 0;

                        if(current.parent.isFood && !p.hadLunch)
                        {
                            if(GetHour() <= 10)
                            {
                                foodImpact = 0.75f;
                            }
                            else if(GetHour() <= 12)
                            {
                                foodImpact = (GetHour() - 10) + 0.5f;
                            }
                            else if(GetHour() <= 14)
                            {
                                foodImpact = 2.5f - (GetHour() - 12);
                            }
                            else if(GetHour() <= 16)
                            {
                                foodImpact = 0.75f;
                            }
                            else if(GetHour() <= 18)
                            {
                                foodImpact = 0.75f;
                            }
                            else
                            {
                                foodImpact = 2f;
                            }
                        }
                        else if (!p.hadLunch)
                        {
                            foodImpact = 0.25f;
                        }

                        // TODO factor line length into this
                        float demand = current.parent.demand * (ridden ? 1f : 0.75f) * (matchesAge ? 1f : 0.5f) * UnityEngine.Random.Range(0.2f, 1f) * foodImpact;

                        if(!ridden)
                        {
                            unriddenRide = true;
                        }

                        if (demand > previousDemand)
                        {
                            highestDemandSoFar = current;
                            previousDemand = demand;
                        }
                    }
                }
                // for highest rated ride
                if(highestDemandSoFar != null)
                {
                    bool highRidden = p.ridesRidden.Contains(highestDemandSoFar);
                    bool highAge = p.adults == highestDemandSoFar.parent.adultRide;

                    if ((highRidden && !highAge) || !unriddenRide)
                    {
                        // reset rides to keep usage random ish
                        p.ridesRidden.Clear();
                    }
                    highestDemandSoFar.lineup.Enqueue(p);
                    p.inLine = true;
                    p.timeInLine = 0;
                    p.previousDemand = highestDemandSoFar.parent.demand;
                    if(highestDemandSoFar.parent.isFood)
                    {
                        p.hadLunch = true;
                    }
                }
            }
            else
            {

            }
        }
    }

    private void UpdateHappieness(float timeInLine, float demand)
    {
        // Debug.Log($"Updating Happieness with time in line {timeInLine} on ride demand {demand}");
        if(avgHappieness == 0)
        {
            Debug.Log("Average = 0");
            avgHappieness = HappienessFunction(timeInLine, demand);
        }
        else
        {
            avgHappieness *= 0.95f;
            avgHappieness += HappienessFunction(timeInLine, demand) * 0.05f;
        }
        // Debug.Log($"Average happieness now {avgHappieness}");
    }

    private float HappienessFunction(float timeInLine, float demand)
    {
        if (timeInLine == 0)
        {
            return demand / 10f;
        }
        else
        {
            return demand / (timeInLine / 10);
        }
    }

    public void setWaterUsage()
    {
        availableWater = totalWater;

        foreach(RuntimeSlide rs in allSlides)
        {
            if(!rs.closed)
            {
                availableWater -= rs.parent.waterDraw;
            }
        }
    }

    #region Time
    private void UpdateTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        //Debug.Log($"Time is {time}, days are {currentTime.Day}");
        //Debug.Log($"Formatted time is {GetHour()}:{GetMinute()} on day {GetDay()}");
    }

    public string GetTime()
    {
        return currentTime.ToString("HH:mm");
    }

    public int GetHour()
    {
        return currentTime.Hour;
    }

    public int GetMinute()
    {
        return currentTime.Minute;
    }

    public int GetDay()
    {
        return currentTime.Day;
    }

    public float GetHourF()
    {
        return (float) GetHour() + ((float) GetMinute() / 60f);
    }

    #endregion
}

[Serializable]
public struct HourTuning
{
    public int spawnRate;
    public int minDurration;
    public int maxDurration;
    public int adultProbability; // [0, 100]
    public bool preLunch;
}