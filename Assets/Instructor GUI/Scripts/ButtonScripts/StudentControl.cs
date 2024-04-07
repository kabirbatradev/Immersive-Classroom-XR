using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class StudentnControl : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject[] studentsHeads;

    // ------------ Button Functions ------------
    // Used to put everyone student in the same group
    public void SplitAll()
    {
        InstructorCloudFunctions.Instance.SetAllStudentsGroupOne();
    }
    // Used to split students into individual groups
    public void SplitIndividual()
    {
        InstructorCloudFunctions.Instance.SetStudentsIntoIndividualGroups();
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
            groupAssignment[index] = row;
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
            int group = row / 2;
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

        List<int[]> HARDCODED = new List<int[]>();
        HARDCODED.Add(new int[] { 1, 1, 2, 2, 3, 3 });
        HARDCODED.Add(new int[] { 200, 200, 1, 1, 2, 2, 3, 3, 200 });
        HARDCODED.Add(new int[] { 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 300 });
        HARDCODED.Add(new int[] { 400, 400, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 400, 400, 400 });

        int index = 0;
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            int col = findCol(student);
            groupAssignment[index] = HARDCODED[row - 1][col - 1];
            index++;
        }

        // printout the hardcoded
        print("HARDCODED: ");
        foreach (int[] row in HARDCODED)
        {
            foreach (int col in row)
            {
                print(col);
            }
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
