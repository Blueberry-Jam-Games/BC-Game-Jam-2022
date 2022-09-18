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

    private void Start()
    {
        for(int i = 0, count = lanesOpen.Count; i < count; i++)
        {
            laneDamage.Add(0);
        }
    }

    public void NotifyRidership(int people)
    {
        float damage = (people * parent.damageMultiplier) / (float)OpenLanes();
        for (int i = 0; i < lanesOpen.Count; i++)
        {
            if(lanesOpen[i])
            {
                laneDamage[i] += damage * UnityEngine.Random.Range(0f, 1f);

                if (laneDamage[i] >= parent.damageThreshold)
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
    }

    private void Update()
    {
        bool fullRepair = true;
        for (int i = 0, count = lanesOpen.Count; i < count; i++)
        {
            if (!lanesOpen[i] && currentStaff > OpenLanes())
            {
                laneDamage[i] -= parent.damageMultiplier * (currentStaff - OpenLanes());
                if(laneDamage[i] <= 0f)
                {
                    laneDamage[i] = 0f;
                }
                else
                {
                    fullRepair = false;
                }
            }
        }

        if (brokenDown && fullRepair)
        {
            brokenDown = false;
        }
    }

    public void closeRide()
    {
        closingSoon = true;
    }

    private int OpenLanes()
    {
        int laneCount = 0;
        foreach(bool b in lanesOpen)
        {
            if(b)
            {
                laneCount += 1;
            }
        }
        return laneCount;
    }

    public void openRide()
    {
        int laneCount = OpenLanes();
        if (!brokenDown)
        {
            if(currentStaff >= laneCount)
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
