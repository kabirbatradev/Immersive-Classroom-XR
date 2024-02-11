using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterControl : MonoBehaviour
{
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

    public void resetTheaterMode()
    {
        StreamTheaterModeData.Instance.ResetTheaterMode();
    }
}
