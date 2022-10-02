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
        if (ride.brokenDown)
        {
            SetChildrenActive(true);
        } else
        {
            SetChildrenActive(false);
        }
    }

    private void SetChildrenActive(bool active)
    {
        for (int i = 0, count = transform.childCount; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(active);
        }
    }
}
