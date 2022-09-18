using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSlide : MonoBehaviour
{
    public GameplayManager gm;

    public Queue<Person> lineup = new Queue<Person>();

    public WaterSlides parent;
    public int currentStaff = 1;
    public float capacityThisTick;

    public bool closingSoon = false;
    public bool closed = false;

    public int lanes;

    [SerializeField]
    public List<bool> lanesOpen = new List<bool>();
    
    [SerializeField]
    public List<float> laneDamage = new List<float>();

    public bool brokenDown = false;
    private float effectiveDamageThreshold = 100f;

    private void Start()
    {
        effectiveDamageThreshold = parent.damageThreshold * UnityEngine.Random.Range(0.125f, 1f);
        for(int i = 0, count = lanesOpen.Count; i < count; i++)
        {
            laneDamage.Add(0);
        }
    }

    public void NotifyRidership(int people)
    {
        float damage = (people * parent.damageMultiplier) / (float)CountOpenLanes();
        for (int i = 0; i < lanesOpen.Count; i++)
        {
            if(lanesOpen[i])
            {
                laneDamage[i] += damage * UnityEngine.Random.Range(0f, 1f);

                if (laneDamage[i] >= effectiveDamageThreshold)
                {
                    Breakdown();
                }
            }
        }
    }

    public void Breakdown()
    {
        Debug.Log($"Ride {name} has broken down");
        brokenDown = true;
        closed = true;
        capacityThisTick = 0f;

        while (lineup.Count > 0)
        {
            Person ps = lineup.Dequeue();
            ps.inLine = false;
        }

        for(int i = 0; i < lanesOpen.Count; i++)
        {
            lanesOpen[i] = false;
        }

        Debug.Log("Lineup cleared");
    }

    private void Update()
    {
        bool fullRepair = true;
        for (int i = 0, count = lanesOpen.Count; i < count; i++)
        {
            if (!lanesOpen[i] && currentStaff > CountOpenLanes())
            {
                laneDamage[i] -= parent.damageMultiplier * (currentStaff - CountOpenLanes()) * parent.repairMultiplier;
                //Debug.Log($"Lane damage {laneDamage[i]}, full repair {fullRepair}");
                if(laneDamage[i] <= 0f)
                {
                    laneDamage[i] = 0f;
                }
                else
                {
                    //Debug.Log("Lane damaged, leaving full repair false");
                    fullRepair = false;
                }
            }
        }

        //Debug.Log($"Repair check, broken down {brokenDown}, repair {fullRepair}");
        if (brokenDown && fullRepair)
        {
            brokenDown = false;
            effectiveDamageThreshold = parent.damageThreshold * UnityEngine.Random.Range(0.125f, 1f);
        }
    }

    public void closeRide()
    {
        closingSoon = true;
    }

    private int CountOpenLanes()
    {
        int laneCount = 0;


            foreach(bool b in lanesOpen)
            {
                if(b)
                {
                    laneCount += 1;
                }
            }
        }
        return laneCount;
    }

    public void openRide()
    {
        int laneCount = CountOpenLanes();
        if (!brokenDown)
        {
            if(currentStaff >= laneCount && parent.waterDraw * laneCount < gm.availableWater)
            {
                if(currentStaff > 0)
                {
                    closed = false;
                    closingSoon = false;
                }
            }
        }
    }

    public void closeLane(int slide)
    {
        if(lanes > 1)
        {
            if(lanesOpen[slide])
            {
                lanesOpen[slide] = false;
            }
        }
    }

    public void openLane(int slide)
    {
        int laneCount = 0;
        foreach(bool b in lanesOpen)
            {
            if(b)
            {
                laneCount += 1;
            }
        }

        if(gm.availableWater - parent.waterDraw >= 0 && currentStaff > laneCount)
        {
            lanesOpen[slide] = true;
        }
    }

    public bool queueCleared()
    {
        if(lineup.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float getCapacity()
    {
        float output = 0;

        int[] staffAssignment = new int[lanes];

        int tempCurrentStaff = currentStaff;

        int a = 0;
        while(tempCurrentStaff > 0)
        {
            if(lanesOpen[a%3])
            {
                staffAssignment[a%3] += 1;
                tempCurrentStaff -= 1;
            }

            a++;
        }

        for(int i = 0; i < lanes; i++)
        {
            if(lanesOpen[i])
            {
                if(staffAssignment[i] < parent.staffVsCapacity.Length)
                {
                    output += parent.staffVsCapacity[staffAssignment[i] - 1];
                }
                else
                {
                    output += parent.staffVsCapacity[parent.staffVsCapacity.Length - 1];
                }
            }
        }

        return output;
    }

    public void addStaff()
    {
        if(currentStaff < parent.maxStaff && gm.staffAvailable >= 1)
        {
            gm.staffAvailable -= 1;
            currentStaff += 1;
        }
    }

    public void removeStaff()
    {
        int laneCount = 0;
        if(closed)
        {
            if(currentStaff > 0)
            {
                currentStaff -= 1;
                gm.staffAvailable += 1;
            }
        }
        else
        {
            foreach(bool b in lanesOpen)
            {
                if(b)
                {
                    laneCount += 1;
                }
            }
            if(currentStaff > laneCount)
            {
                gm.staffAvailable += 1;
                currentStaff -= 1;
            }
        }
    }
}
