using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSlide : MonoBehaviour
{
    public GameplayManager gm;
    public float damage;
    // list people lineup
    public Queue<Person> lineup = new Queue<Person>();

    public WaterSlides parent;
    public int currentStaff = 1;
    public float capacityThisTick;

    public bool closingSoon = false;

    public bool closed = false;

    public int lanes;

    [SerializeField]
    public List<bool> lanesOpen = new List<bool>();

    // list staff
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void closeRide()
    {
        closingSoon = true;
    }

    public void openRide()
    {
        int laneCount = 0;
        foreach(bool b in lanesOpen)
        {
            if(b)
            {
                laneCount += 1;
            }
        }
        if(currentStaff >= laneCount)
        {
            if(currentStaff > 0)
            {
                closed = false;
                closingSoon = false;
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
