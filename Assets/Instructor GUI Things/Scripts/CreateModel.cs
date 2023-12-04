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
    public int totalNumberOfModels;

    static private int modelIndex = -1;

    // call back func for creating model
    public void createModel()
    {
        // hide createModelBtn
        createModelBtn.SetActive(false);  

        // Show the other btns
        otherModelBtns.SetActive(true);

        // create model code here
        Debug.Log("Create model button pressed");
        // call the function from instructor cloud functions 
        InstructorCloudFunctions.Instance.CreateMainObjectContainerPerGroup();

        // The following code changes the placeholder text
        modelIndex = 1;
        changePlaceHolderText();
        
    }

    // call back func for changing model
    public void changeModel()
    {

        // if (modelIndex < totalNumberOfModels)
        // {
        //     modelIndex++;
        // } else
        // {
        //     modelIndex = 1;
        // }

        totalNumberOfModels = InstructorCloudFunctions.Instance.getTotalNumberOfModels();

        modelIndex++;
        if (modelIndex > totalNumberOfModels) {
            modelIndex = 1;
        }


        // Implement change model code here
        Debug.Log("Change model button pressed");
        // call the function from instructor cloud functions 
        InstructorCloudFunctions.Instance.SetActiveModelNumber(modelIndex);
        

        changePlaceHolderText();
    }


    // call back func for killing model
    public void killModel()
    {
        // destroy model code here
        Debug.Log("kill model button pressed");
        // call the function from instructor cloud functions 
        InstructorCloudFunctions.Instance.DeleteAllMainObjects();

        // hide other btns
        otherModelBtns.SetActive(false);

        // replace with createModel btn
        createModelBtn.SetActive(true);

        // The following code changes the placeholder text
        modelIndex = -1;
        changePlaceHolderText() ;
    }

    // used for changing the model placeholder text
    private void changePlaceHolderText()
    {
        GameObject panelText = modelPanel.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = panelText.GetComponent<TextMeshProUGUI>();
        if (modelIndex == -1)
        {
            text.text = "No Model";
            return;
        }
        text.text = "Model: " + modelIndex;
    }

    // used to get the current model number
    public static int getCurrentModelNum()
    {
        return modelIndex;
    }



}
