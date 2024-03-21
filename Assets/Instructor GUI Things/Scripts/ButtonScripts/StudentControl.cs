using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StudentnControl : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject[] studentsHeads;

    // ------------ Button Functions ------------
    // Used to split students into individual groups
    public void SplitIndividual()
    {
        // Find all markers and students
        // This is set during runtime so that the markers and students are found after they are created
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        int[] groupAssignment = new int[studentsHeads.Length];
        int group = 1;
        Debug.Log("Len of studentsHeads: " + studentsHeads.Length);
        foreach (GameObject student in studentsHeads)
        {
            // append current group to groupAssignment
            groupAssignment[group - 1] = group;
            group++;
        }
        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);
    }

    // Used to split students into rows
    public void SplitRow()
    {
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        int[] groupAssignment = new int[studentsHeads.Length];
        int index = 0;
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            Debug.Log("Row: " + row);
            groupAssignment[index] = row + 1;
            index++;
        }
        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);
    }

    // Used to split students int two rows
    public void SplitDoubleRow()
    {
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        int[] groupAssignment = new int[studentsHeads.Length];
        int index = 0;
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            Debug.Log("Row: " + row);
            int group = row / 2 + 1;
            groupAssignment[index] = group;
            index++;
        }
        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);
    }

    // Used to split students into squares of 4
    public void SplitFour()
    {
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        int[] groupAssignment = new int[studentsHeads.Length];
        int group = 1;
        Debug.Log("Len of studentsHeads: " + studentsHeads.Length);
        foreach (GameObject student in studentsHeads)
        {
            // append current group to groupAssignment
            groupAssignment[group - 1] = group;
            group++;
        }
        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);
    }
    // ------------ Button Functions ------------



    // ------------ Helper Functions ------------
    // Find the closest marker to the student and return the row
    public int findRow(GameObject studentsHeads)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(studentsHeads.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<SeatMarkerData>().GetRow();
    }

    // Find the closest marker to the student and return the column
    public int findCol(GameObject studentsHeads)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(studentsHeads.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<SeatMarkerData>().GetColumn();
    }

    // Find the closest marker to the student and return the group
    public int findFourGroup(GameObject studentsHeads)
    {
        return 1;
    }
    // ------------ Helper Functions ------------
}
