using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSlide : MonoBehaviour
{
    public GameplayManager gm;

    public string slideName;

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

    [SerializeField]
    public List<GameObject> images = new List<GameObject>();

    private void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameplayManager>();
        effectiveDamageThreshold = parent.damageThreshold * UnityEngine.Random.Range(0.125f, 1f);
        for(int i = 0, count = lanesOpen.Count; i < count; i++)
        {
            laneDamage.Add(0);
        }

        UpdateRideMaterials();
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
        Debug.Log("Close");
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
        return laneCount;
    }

    public void openRide()
    {
        int laneCount = CountOpenLanes();
        if (!brokenDown)
        {
            if(currentStaff >= laneCount && parent.waterDraw * laneCount < gm.availableWater && laneCount > 0)
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

        UpdateRideMaterials();

        // foreach(GameObject go in images)
        // {
        //     if(slide == 0)
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide1", 0);
        //     }
        //     else if(slide == 1)
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide2", 0);
        //     }
        //     else
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide3", 0);
        //     }
        // }
    }

    public void openLane(int slide)
    {
        int laneCount = CountOpenLanes();

        if(gm.availableWater - parent.waterDraw >= 0 && currentStaff > laneCount)
        {
            lanesOpen[slide] = true;
        }

        UpdateRideMaterials();

        // foreach(GameObject go in images)
        // {
        //     if(slide == 0)
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide1", 1);
        //     }
        //     else if(slide == 1)
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide2", 1);
        //     }
        //     else
        //     {
        //         go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide3", 1);
        //     }
        // }
    }

    private void UpdateRideMaterials()
    {
        for (int i = 0, count = 3; i < count; i++)
        {
            foreach(GameObject go in images)
            {
                if(i == 0 && i < lanes)
                {
                    go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide1", lanesOpen[i] ? 1 : 0);
                }
                else if(i == 1 && i < lanes)
                {
                    go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide2", lanesOpen[i] ? 1 : 0);
                }
                else if (i == 2 && i < lanes)
                {
                    go.GetComponent<Renderer>().sharedMaterial.SetInt("_Slide3", lanesOpen[i] ? 1 : 0);
                }
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
        int stopNow = 0;
        while(tempCurrentStaff > 0)
        {
            if(lanesOpen[a % lanes])
            {
                staffAssignment[a % lanes] += 1;
                tempCurrentStaff -= 1;
                stopNow = 0;
            }
            else
            {
                stopNow += 1;
            }

            if(stopNow > 4)
            {
                break;
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
            laneCount = CountOpenLanes();
            if(currentStaff > laneCount)
            {
                gm.staffAvailable += 1;
                currentStaff -= 1;
            }
        }
    }
}
