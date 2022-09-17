using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSlide : MonoBehaviour
{
    public float damage;
    // list people lineup
    public Queue<Person> lineup = new Queue<Person>();

    public WaterSlides parent;
    public int currentStaff;
    public float capacityThisTick;

    // list staff
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
