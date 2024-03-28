using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TheaterRetract : MonoBehaviour
{
    public float ceilingPercent = 0.0f;
    public float wallPercent = 0.0f;
    public GameObject[] ceilings;
    public GameObject[] walls;

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
        foreach (GameObject ceiling in ceilings)
        {
            ceiling.transform.localPosition = new Vector3(ceiling.transform.localPosition.x +
                (ceiling.transform.localPosition.x / Math.Abs(ceiling.transform.localPosition.x * 10 * ceilingPercent)),
            ceiling.transform.localPosition.y + wallPercent * -20.0f,
            ceiling.transform.localPosition.z);
        }
        foreach (GameObject wall in walls)
        {
            wall.transform.localPosition = new Vector3(
                wall.transform.localPosition.x,
                wall.transform.localPosition.y + wallPercent * -40.0f,
                wall.transform.localPosition.z);
        }
    }
}
