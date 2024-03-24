using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterControl : MonoBehaviour
{
    private bool theaterStarted = false;
    private bool theaterGoing = false;
    public void toggleTheater()
    {
        if (theaterStarted)
        {
            if (theaterGoing)
            {
                pauseTheaterMode();
                theaterGoing = false;
            }
            else
            {
                continueTheaterMode();
                theaterGoing = true;
            }
        }
        else
        {
            startTheaterMode();
            theaterStarted = true;
            theaterGoing = true;
        }
    }

    public void resetTheaterMode()
    {
        StreamTheaterModeData.Instance.ResetTheaterMode();
        theaterStarted = false;
        theaterGoing = false;
    }

    public void startTheaterMode()
    {
        StreamTheaterModeData.Instance.TriggerTheaterMode();
    }

    public void pauseTheaterMode()
    {
        StreamTheaterModeData.Instance.PauseTheaterMode();
    }

    public void continueTheaterMode()
    {
        StreamTheaterModeData.Instance.ContinueTheaterMode();
    }
}
