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

        for(int i = 0; i < currentStaff; i++)
        {
            staffAssignment[i%lanes] += 1;
        }

        for(int i = 0; i < lanes; i++)
        {
            if(lanesOpen[i])
            {
                output += parent.staffVsCapacity[staffAssignment[i] - 1];
            }
        }

        return output;
    }

    public void addStaff()
    {
        if(currentStaff <= parent.maxStaff && gm.staffAvailable >= 1)
        {
            gm.staffAvailable -= 1;
            currentStaff += 1;
        }
    }

    public void removeStaff()
    {
        int laneCount = 0;
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
