using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CreateModel : MonoBehaviour
{
    public GameObject createModelBtn;
    public GameObject otherModelBtns;
    public GameObject modelPanel;
    public int numModel;

    static private int count = -1;

    // call back func for creating model
    public void createModel()
    {
        // hide createModelBtn
        createModelBtn.SetActive(false);  

        // Show the other btns
        otherModelBtns.SetActive(true);

        //TODO: create model code here


        // The following code changes the placeholder text
        count = 0;
        changePlaceHolderText();
        
    }

    // call back func for changing model
    public void changeModel()
    {
        //TODO: Implement change model code here

        // The following code changes the placeholder text
        if (count < numModel)
        {
            count++;
        } else
        {
            count = 0;
        }

        changePlaceHolderText();
    }


    // call back func for killing model
    public void killModel()
    {
        // TODO: destroy model code here

        // hide other btns
        otherModelBtns.SetActive(false);

        // replace with createModel btn
        createModelBtn.SetActive(true);

        // The following code changes the placeholder text
        count = -1;
        changePlaceHolderText() ;
    }

    // used for changing the model placeholder text
    private void changePlaceHolderText()
    {
        GameObject panelText = modelPanel.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = panelText.GetComponent<TextMeshProUGUI>();
        if (count == -1)
        {
            text.text = "No Model";
            return;
        }
        text.text = "Model: " + count;
    }

    // used to get the current model number
    public static int getCurrentModelNum()
    {
        return count;
    }



}
