using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TheaterRetract : MonoBehaviour
{
    public float ceilingPercent = 0.0f;
    public float wallPercent = 0.0f;
    public GameObject percentageDisplay;

    public void Update()
    {
        if (ceilingPercent >= 1.0f)
        {
            ceilingPercent = 1.0f;
        }
        if (wallPercent >= 1.0f)
        {
            wallPercent = 1.0f;
        }
        ceilingPercent = StreamTheaterModeData.Instance.ceilingRemovedPercentage;
        wallPercent = StreamTheaterModeData.Instance.wallLoweredPercentage;
        retract(ceilingPercent, wallPercent);
    }

    public void retract(float ceilingPercent, float wallPercent)
    {
        // set percentage display text
        percentageDisplay.GetComponent<TextMeshProUGUI>().text = "Ceiling: " + (ceilingPercent * 100).ToString("F0") + "%\n" + "Walls: " + (wallPercent * 100).ToString("F0") + "%";
    }
}
