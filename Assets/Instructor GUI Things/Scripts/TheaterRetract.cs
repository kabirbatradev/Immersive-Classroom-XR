using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterRetract : MonoBehaviour
{
    public float ceilingPercent = 0.0f;
    public float wallPercent = 0.0f;
    public GameObject[] wallsO;
    public GameObject[] wallsF;
    private Vector3 wallOStartHeight;
    private Vector3 wallFStartHeight;
    private Vector3 ceilingStartScale;
    // how much should wallsO and wallsF move in the y direction
    public float wallYChanges = -3.0f;
    public GameObject ceilingO;
    public GameObject ceilingF;
    public float ceilingFullSize = 6.0f;
    public void Start()
    {
        wallOStartHeight = wallsO[0].transform.position;
        wallFStartHeight = wallsF[0].transform.position;
        ceilingStartScale = ceilingO.transform.localScale;
    }

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
        retractWalls();
        retractCeiling();
    }

    public void retractWalls()
    {
        foreach (GameObject wall in wallsO)
        {
            wall.transform.position = new Vector3(wall.transform.position.x,
            wallOStartHeight.y + wallPercent * wallYChanges,
            wall.transform.position.z);
        }
        foreach (GameObject wall in wallsF)
        {
            wall.transform.position = new Vector3(wall.transform.position.x,
            wallFStartHeight.y + wallPercent * wallYChanges,
            wall.transform.position.z);
        }
    }

    public void retractCeiling()
    {
        // change scale
        ceilingF.transform.localScale = new Vector3(ceilingF.transform.localScale.x,
        ceilingPercent * ceilingFullSize,
        ceilingPercent * ceilingFullSize);
    }
}
