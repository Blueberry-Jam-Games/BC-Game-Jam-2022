using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunVisual : MonoBehaviour
{
    public Light sun;
    public GameObject sunRotation;
    public Light moon;
    public GameObject moonRotation;
    
    public GameplayManager gm;

    public float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameplayManager>();
    }

    // Update is called once per frame
    void Update()
    {
        time = (gm.GetHourF()%24) * 10;

       sunRotation.transform.rotation = Quaternion.Euler(time * 1.5f - 180, 60f, 0f);
        float temp = ((-14) * time * time) / 9;
        temp += (1120 * time) / 3;
        temp -= 14800;

        if(temp < 2000)
        {
            temp = 2000;
        }

        sun.colorTemperature = temp;

        if(time < 60 || time > 180)
        {
            sun.intensity = 0;
        }
        else 
        {
            sun.intensity = 1;
        }

        if(time < 80)
        {
            moonRotation.transform.rotation = Quaternion.Euler(30f, time * 0.5f, 0f);
        }
        else if(time > 160)
        {
            moonRotation.transform.rotation = Quaternion.Euler(30f, (time - 240) * 0.5f, 0f);
        }
        
        if(time < 60)
        {
            moon.intensity = 1;
        }
        else if(time < 80)
        {
            moon.intensity = 1 - ((time - 60) / 20);
        }
        else if(time < 160)
        {
            moon.intensity = 0;
        }
        else if(time < 180)
        {
            moon.intensity = (time - 160) / 20;
        } else
        {
            moon.intensity = 1;
        }
    }
}
