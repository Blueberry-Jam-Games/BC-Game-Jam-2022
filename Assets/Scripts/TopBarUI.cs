using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopBarUI : MonoBehaviour
{
    public TextMeshProUGUI totalPeople;
    public TextMeshProUGUI totalWater;
    public TextMeshProUGUI totalStaff;
    public TextMeshProUGUI happiness;
    public TextMeshProUGUI time;

    public GameplayManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindWithTag("GameController").GetComponent<GameplayManager>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        totalStaff.text = $"Staff: {gm.staffAvailable}";
        totalWater.text = $"Available Water: {gm.availableWater}";
        time.text = gm.GetTime();
        totalPeople.text = $"Total Guest: {gm.GetTotalPeople()}";
        happiness.text = $"Happiness Score: {gm.avgHappieness}";

    }
}
