using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Time Management")]
    [SerializeField]
    private float timeMultiplier = 360f;

    private DateTime currentTime;

    [Header("Tuning")]
    public List<HourTuning> hours = new List<HourTuning>(24);

    public List<Person> allPeople = new List<Person>(1000);

    private List<RuntimeSlide> allSlides = new List<RuntimeSlide>();

    void Start()
    {
        Debug.Log("Start");
        currentTime = new DateTime(2022, 08, 01, 7, 0, 0);
        GameObject[] slides = GameObject.FindGameObjectsWithTag("WaterSlide");
        foreach(GameObject go in slides)
        {
            allSlides.Add(go.GetComponent<RuntimeSlide>());
        }
    }
    
    private void FixedUpdate()
    {
        CreateNewPeople();
    }

    void Update()
    {
        UpdateTime();
        FlagTimedOut();
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
    
    List<Person> tagRemoval = new List<Person>();

    private void FlagTimedOut()
    {
        for (int i = 0, count = allPeople.Count; i < count; i++)
        {
            Person current = allPeople[i];
            TimeSpan length = currentTime.Subtract(current.startTime);
            if (length.TotalMinutes > current.totalTime)
            {
                tagRemoval.Add(current);
            }
        }
    }

    public void ProcessLineups()
    {
        // Process each water slide removing people from the queues
        foreach(RuntimeSlide rs in allSlides)
        {
            rs.capacityThisTick += rs.parent.staffVsCapacity[rs.currentStaff];

            while(rs.lineup.Count > 0 && rs.capacityThisTick > rs.lineup.Peek().partySize)
            {
                Person serverd = rs.lineup.Dequeue();
                serverd.inLine = false;
                if (!serverd.ridesRidden.Contains(rs))
                {
                    serverd.ridesRidden.Add(rs);
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
        for(int t = 0, cnt = allPeople.Count; t < cnt; t++)
        {
            Person p = allPeople[t];
            if(!p.inLine)
            {
                RuntimeSlide highestDemandSoFar = null;
                bool unriddenRide = false;
                for(int i = 0, count = allSlides.Count; i < count; i++)
                {
                    RuntimeSlide current = allSlides[i];
                    bool ridden = p.ridesRidden.Contains(current);
                    bool matchesAge = p.adults == current.parent.adultRide;
                    
                    float demand = current.parent.demand * (ridden ? 1f : 0.75f) * (matchesAge ? 1f : 0.5f) * UnityEngine.Random.Range(0f, 1f);
                    float currentDemand = highestDemandSoFar?.parent?.demand ?? 0f;

                    if(!ridden)
                    {
                        unriddenRide = true;
                    }

                    if (demand > currentDemand)
                    {
                        highestDemandSoFar = current;
                    }
                }
                // for highest rated ride
                bool highRidden = p.ridesRidden.Contains(highestDemandSoFar);
                bool highAge = p.adults == highestDemandSoFar.parent.adultRide;
                if ((highRidden && !highAge) || !unriddenRide)
                {
                    // reset rides to keep usage random ish
                    p.ridesRidden.Clear();
                }
                highestDemandSoFar.lineup.Enqueue(p);
                p.inLine = true;
            }
        }
    }

    #region Time
    private void UpdateTime()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        string time = currentTime.ToString("HH:mm");
        //Debug.Log($"Time is {time}, days are {currentTime.Day}");
        //Debug.Log($"Formatted time is {GetHour()}:{GetMinute()} on day {GetDay()}");
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