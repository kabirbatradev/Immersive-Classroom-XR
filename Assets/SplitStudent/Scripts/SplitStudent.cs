using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitStudent : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject[] studentsHeads;
    public int[] groupAssignment;

    // ------------ Button Functions ------------
    // Used to split students into individual groups
    public void SplitIndividual()
    {
        // Find all markers and students
        // This is set during runtime so that the markers and students are found after they are created
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        int group = 1;
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
        markers = GameObject.FindGameObjectsWithTag("Marker");
        studentsHeads = GameObject.FindGameObjectsWithTag("FakeStudent");
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            student.GetComponent<FakeStudent>().group = row + 1;
        }
    }

    // Used to split students int two rows
    public void SplitDoubleRow()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        studentsHeads = GameObject.FindGameObjectsWithTag("FakeStudent");
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            student.GetComponent<FakeStudent>().group = (row / 2) + 1;
        }
    }

    // Used to split students into squares of 4
    public void SplitFour()
    {
    }
    // ------------ Button Functions ------------



    // ------------ Helper Functions ------------
    // Find the closest marker to the student and return the row
    public int findRow(GameObject studentsHeads)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(studentsHeads.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetRow();
    }

    // Find the closest marker to the student and return the column
    public int findCol(GameObject studentsHeads)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(studentsHeads.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetColumn();
    }

    // Find the closest marker to the student and return the group
    public int findFourGroup(GameObject studentsHeads)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(studentsHeads.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetGroup();
    }
    // ------------ Helper Functions ------------
}
