using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SplitStudentController : MonoBehaviour
{
    public GameObject firstLevelUI;
    public GameObject splitStudentUI;
    public GameObject studentPanel;

    // call back fuc for split student btn
    public void enterSplitMode()
    {
        // hide first level UI
        firstLevelUI.SetActive(false);

        // replace with splitStudentUI
        splitStudentUI.SetActive(true);

    }

    // call back func for splitting student into individual groups
    public void splitIndividual()
    {
        // code here to tell the server splitting into groups of 1 student
        Debug.Log("split individual button pressed");
        InstructorCloudFunctions.Instance.SetStudentsIntoIndividualGroups();

        // go back to first level
        goBack();


        // change placeholder text
        changePlaceHolderText(1);
    }

    public void splitPair()
    {
        // code here to tell the server splitting into groups of 2 students
        Debug.Log("split into pairs button pressed");
        InstructorCloudFunctions.Instance.SetStudentsIntoGroupsOfTwo();

        // go back to first level
        goBack();

        // change placeholder text
        changePlaceHolderText(2);

    }

    public void splitAll()
    {
        // code here to tell the server to have a group of all students
        Debug.Log("all students 1 group button pressed");
        InstructorCloudFunctions.Instance.SetAllStudentsGroupOne();

        goBack();
        changePlaceHolderText(0);
    }

    public void goBack()
    {
        splitStudentUI.SetActive(false);
        firstLevelUI.SetActive(true);
    }

    private void changePlaceHolderText(int numStudent)
    {

        TextMeshProUGUI studentPanelText = studentPanel.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        if (numStudent == 0)
        {
            studentPanelText.text = "all students are in the same group";
            return;
        }
        studentPanelText.text = "Split students into group of " + numStudent;

    }

}
