using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateInput : MonoBehaviour
{
    public TheaterRetract theaterRetract;
    public float wallTimer = 0;
    public float ceilingTimer = 0;
    public float finishedTime = 10;
    public bool startCeilingTimer = false;

    public void timerButton()
    {
        startCeilingTimer = true;
    }
    public void Update()
    {
        if (startCeilingTimer)
        {
            ceilingTimer += Time.deltaTime;
            theaterRetract.ceilingPercent = ceilingTimer / finishedTime;
            theaterRetract.retractCeiling();
        }
        if (ceilingTimer / finishedTime >= 1)
        {
            wallTimer += Time.deltaTime;
            theaterRetract.wallPercent = wallTimer / finishedTime;
            theaterRetract.retractWalls();
        }
    }
}
