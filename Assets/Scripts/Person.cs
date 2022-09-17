using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct Person
{
    public bool adults; // else kids
    public int totalTime; // in minutes
    public bool hadLunch; // true if past lunch time on creation
    public int partySize;
    public bool inLine;
    public DateTime startTime;
    public HashSet<RuntimeSlide> ridesRidden;

    public Person(bool adults, int totalTime, bool lunch, int partySize, DateTime startTime)
    {
        this.adults = adults;
        this.totalTime = totalTime;
        this.hadLunch = lunch;
        this.startTime = startTime;
        this.partySize = partySize;
        inLine = false;
        ridesRidden = new HashSet<RuntimeSlide>();
    }
}
