using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideAndShowUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftHideBtn;
    public GameObject rightHideBtn;
    public List<GameObject> leftUIToHide;
    public List<GameObject> rightUIToHide;

    private bool leftIsHidden = false;
    private bool rightIsHidden = false;


    public void leftBtnClick()
    {
        if (leftIsHidden)
        {
            showLeftUI();
        } else
        {
            hideLeftUI();
        }
    }

    public void rightBtnClick()
    {
        if (rightIsHidden)
        {
            showRightUI();
        } else
        {
            hideRightUI();
        }
    }

    private void hideLeftUI()
    {
        // hide the ui on the left
        foreach (GameObject element in leftUIToHide)
        {
            element.SetActive(false);
        }

        // change the text on the btn from 'hide' to 'show'
        changeBtnText("Show", leftHideBtn);
        leftIsHidden = true;
    }

    private void showLeftUI()
    {
        // show the ui on the left
        foreach (GameObject element in leftUIToHide)
        {
            element.SetActive(true);
        }
        // change the text on the btn from 'show' to 'hide'
        changeBtnText("Hide", leftHideBtn);
        leftIsHidden = false;
    }

    private void hideRightUI()
    {
        // hide the ui on the right
        foreach (GameObject element in rightUIToHide)
        {
            element.SetActive(false);
        }

        // change the text on the btn from 'hide' to 'show'
        changeBtnText("Show", rightHideBtn);
        rightIsHidden = true;
    }

    private void showRightUI()
    {
        // show the ui on the left
        foreach (GameObject element in rightUIToHide)
        {
            element.SetActive(true);
        }
        // change the text on the btn from 'show' to 'hide'
        changeBtnText("Hide", rightHideBtn);
        rightIsHidden = false;
    }
    private void changeBtnText(string newText, GameObject btn)
    {
        GameObject panelText = btn.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = panelText.GetComponent<TextMeshProUGUI>();
        text.text = newText;
    }

}
