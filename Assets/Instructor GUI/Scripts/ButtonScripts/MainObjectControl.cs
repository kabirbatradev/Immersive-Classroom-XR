using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CreateModel : MonoBehaviour
{
    public int totalNumberOfModels;
    public RuntimeGizmo gizmo;

    static private int modelIndex = -1;
    public GameObject mainObjectContainer;

    // call back func for creating model
    public void createModel()
    {
        // create model code here
        Debug.Log("Create model button pressed");
        // call the function from instructor cloud functions
        InstructorCloudFunctions.Instance.CreateMainObjectContainerPerGroup();

        modelIndex = 1;
        InstructorCloudFunctions.Instance.SetActiveModelNumber(modelIndex);

        if (mainObjectContainer != null)
        {
            mainObjectContainer.transform.position = new Vector3(0, 0, 0);
            for (int i = 0; i < mainObjectContainer.transform.childCount; i++)
            {
                if (i == modelIndex - 1)
                {
                    mainObjectContainer.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    mainObjectContainer.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    // call back func for changing model
    public void changeModel()
    {
        totalNumberOfModels = InstructorCloudFunctions.Instance.getTotalNumberOfModels();

        modelIndex++;
        if (modelIndex > totalNumberOfModels)
        {
            modelIndex = 1;
        }

        // Implement change model code here
        Debug.Log("Change model button pressed");
        // call the function from instructor cloud functions
        InstructorCloudFunctions.Instance.SetActiveModelNumber(modelIndex);

        if (mainObjectContainer != null)
        {
            for (int i = 0; i < mainObjectContainer.transform.childCount; i++)
            {
                if (i == modelIndex - 1)
                {
                    mainObjectContainer.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    mainObjectContainer.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }


    // call back func for killing model
    public void killModel()
    {
        // destroy model code here
        Debug.Log("kill model button pressed");
        // call the function from instructor cloud functions
        InstructorCloudFunctions.Instance.DeleteAllMainObjects();

        // The following code changes the placeholder text
        modelIndex = -1;

        if (mainObjectContainer != null)
        {
            for (int i = 0; i < mainObjectContainer.transform.childCount; i++)
            {
                mainObjectContainer.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void toggleGizmo()
    {
        gizmo.isGizmoActive = !gizmo.isGizmoActive;
    }
}
