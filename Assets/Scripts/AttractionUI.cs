using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttractionUI : MonoBehaviour
{
    public TextMeshProUGUI rideName;
    public TextMeshProUGUI waterUsage;
    public TextMeshProUGUI staff;
    public Button upButton;
    public Button downButton;
    public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public Toggle toggle1;
    public Toggle toggle2;
    public Toggle toggle3;
    

    // Start is called before the first frame update
    void Start()
    {
        rideName.text = "name";
        waterUsage.text = "Water Usage: 1";
        staff.text = "1";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
