using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaterSlide", menuName = "BJG/Water Slide", order = 1)]
public class WaterSlides : ScriptableObject
{
    public int maxStaff;
    public float[] staffVsCapacity;
    public float damageThreshold;
    public float damageMultiplier;
    public float repairMultiplier;
    public float waterDraw;
    public float demand;
    public bool adultRide;
    public bool isFood;
}
