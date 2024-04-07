using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterControl : MonoBehaviour
{
    // private bool theaterStarted = false;
    // private bool theaterGoing = false;
    public void toggleTheater()
    {


        // if (!theaterStarted)
        // {
        //     StreamTheaterModeData.Instance.TriggerTheaterMode();
        //     theaterStarted = true;
        // }
        // else
        // {
        //     if (theaterGoing)
        //     {
        //         StreamTheaterModeData.Instance.PauseTheaterMode();
        //         theaterGoing = false;
        //     }
        //     else
        //     {
        //         StreamTheaterModeData.Instance.ContinueTheaterMode();
        //         theaterGoing = true;
        //     }
        // }

        StreamTheaterModeData.ToggleTheaterMode();

        GameObject[] streamObjects = GameObject.FindGameObjectsWithTag("StreamTheaterModeData");
        Debug.Log("toggle debug log stream objects length (should be 1): " + streamObjects.Length);
    }

    public void resetTheaterMode()
    {
        // StreamTheaterModeData.Instance.ResetTheaterMode();
        // theaterStarted = false;
        // theaterGoing = false;

        StreamTheaterModeData.ResetTheaterMode();

    }

    public void changeSkybox()
    {
        StreamTheaterModeData.ChangeSkybox();
    }

    // public void startTheaterMode()
    // {
    //     StreamTheaterModeData.Instance.TriggerTheaterMode();
    // }

    // public void pauseTheaterMode()
    // {
    //     StreamTheaterModeData.Instance.PauseTheaterMode();
    // }

    // public void continueTheaterMode()
    // {
    //     StreamTheaterModeData.Instance.ContinueTheaterMode();
    // }
}
