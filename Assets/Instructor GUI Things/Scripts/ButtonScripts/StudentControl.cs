using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SplitStudentController : MonoBehaviour
{
    // call back func for splitting student into individual groups
    public void splitIndividual()
    {
        // code here to tell the server splitting into groups of 1 student
        Debug.Log("split individual button pressed");
        InstructorCloudFunctions.Instance.SetStudentsIntoIndividualGroups();
    }

    public void splitPair()
    {
        // code here to tell the server splitting into groups of 2 students
        Debug.Log("split into pairs button pressed");
        InstructorCloudFunctions.Instance.SetStudentsIntoGroupsOfTwo();
    }

    public void splitAll()
    {
        // code here to tell the server to have a group of all students
        Debug.Log("all students 1 group button pressed");
        InstructorCloudFunctions.Instance.SetAllStudentsGroupOne();
    }
}
