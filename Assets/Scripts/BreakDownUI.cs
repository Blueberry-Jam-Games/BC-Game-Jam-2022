using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BreakDownUI : MonoBehaviour
{
    public RuntimeSlide ride;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();   
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ride.brokenDown) {
            this.gameObject.SetActive(true);
        } else
        {
            this.gameObject.SetActive(false);
        }
    }
}
