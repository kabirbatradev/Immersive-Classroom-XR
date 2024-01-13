using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TheaterController : MonoBehaviour
{
    public GameObject theaterOptionUI;
    public GameObject firstLevelUI;
    public GameObject startTheaterBtn;
    public GameObject stopTheaterBtn;

    private int selected = -1;


    public void enterTheaterMode()
    {
        // disable theater button
        firstLevelUI.SetActive(false);

        // enable theater option ui
        theaterOptionUI.SetActive(true);
    }

    public void selectTheaterMode()
    {
        // if any skybox material is selected,
        // triggers the code for 360 theater
        if (selected != -1)
        {
            // TODO: replace the print statement with 360 theater code
            print(selected);

            // replace startTheaterBtn with StopTheaterBtn
            startTheaterBtn.SetActive(false);
            stopTheaterBtn.SetActive(true);

            // change the button text
            TextMeshProUGUI stopTheaterBtnText = stopTheaterBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            stopTheaterBtnText.text = "Stop Theater Mode: " + selected.ToString(); 

        }

        // disable theater option ui
        theaterOptionUI.SetActive(false);

        // enable first level UI
        firstLevelUI.SetActive(true);

        // reset selected index
        selected = -1;

    }

    public void stopTheaterMode()
    {
        selected = -1;

        // replace the stopTheaterBtn with startTheaterBtn
        stopTheaterBtn.SetActive(false) ;
        startTheaterBtn.SetActive(true) ;

        // TODO: add code here to go from theater mode to regular mode
        print("exiting theater mode: " + selected.ToString());
    }

    // callback func for the first skybox material button
    public void selectFirst()
    {
        selected = 1;
        selectTheaterMode();
    }
    // callback func for the second skybox material button
    public void selectSecond()
    {
        selected = 2;
        selectTheaterMode();
    }
    // callback func for the third skybox material button
    public void selectThird()
    {
        selected = 3;
        selectTheaterMode();
    }
    // callback func for the go back button
    public void goBack()
    {
        selectTheaterMode();
    }



}
