using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RideLabelUI : MonoBehaviour
{
    public RuntimeSlide ride;
    public TextMeshProUGUI rideName;
    public TextMeshProUGUI lineLength;

    // Start is called before the first frame update
    void Start()
    {
        rideName.text = ride.slideName;
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        lineLength.text = $"Parties in line: {ride.lineup.Count}";
    }
}
