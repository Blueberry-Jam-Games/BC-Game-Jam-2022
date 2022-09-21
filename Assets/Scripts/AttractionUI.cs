using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttractionUI : MonoBehaviour
{
    public RuntimeSlide ride;
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

    public Toggle openToggle;
    

    // Start is called before the first frame update
    void Start()
    {
        rideName.text = ride.slideName;
        waterUsage.text = $"Water Usage: {ride.parent.waterDraw}";
        DrawLines();
        UpdateUI();
        upButton.onClick.AddListener(IncreaseStaff);
        downButton.onClick.AddListener(DecreaseStaff);
        toggle1.onValueChanged.AddListener(openLine1);
        toggle2.onValueChanged.AddListener(openLine2);
        toggle3.onValueChanged.AddListener(openLine3);
        openToggle.onValueChanged.AddListener(openRide);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI() 
    {
        staff.text = ride.currentStaff.ToString();
    }

    void DrawLines()
    {
        List<GameObject> lanes = new List<GameObject>(){line1, line2, line3};
        for (int i = 0; i<3; i++) {
            if (i > ride.lanes - 1) {
                lanes[i].SetActive(false);
            }
        }
    }

    void IncreaseStaff()
    {
        ride.addStaff();
    }  

    void DecreaseStaff()
    {
        ride.removeStaff();
    }

    void openLine1(bool open)
    {
        if (open) 
        {
            ride.openLane(0);
        } else 
        {
            ride.closeLane(0);
        }
    }

    void openLine2(bool open)
    {
        if (open) 
        {
            ride.openLane(1);
        } else 
        {
            ride.closeLane(1);
        }
    }

    void openLine3(bool open)
    {
        if (open) 
        {
            ride.openLane(2);
        } else 
        {
            ride.closeLane(2);
        }
    }

    void openRide(bool open)
    {
        Debug.Log("ChangeClose");
        if (open)
        {
            ride.openRide();
        } else
        {
            ride.closeRide();
        }
    }
}
