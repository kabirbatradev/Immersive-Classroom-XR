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
            int row = findRow(student) - 1;
            int group = (row / 2) + 1;
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
        Debug.Log("Number of students: " + studentsHeads.Length);
        int[] groupAssignment = new int[studentsHeads.Length];
        List<List<GameObject>> dynamicArray = new List<List<GameObject>>();
        Dictionary<int, List<GameObject>> rowDictionary = new Dictionary<int, List<GameObject>>();

        // Group students by rows
        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            if (!rowDictionary.ContainsKey(row))
            {
                rowDictionary[row] = new List<GameObject>();
            }
            rowDictionary[row].Add(student);
        }

        // Sort each row based on their z position and add to dynamicArray
        foreach (var row in rowDictionary)
        {
            row.Value.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
            dynamicArray.Add(row.Value);
        }

        List<GameObject> groupList = new List<GameObject>();
        int groupNumber = 1;
        int studentsPerGroup = 4;
        int groupIndex = 0;

        // Assign groups
        while (groupList.Count < studentsHeads.Length)
        {
            for (int i = 0; i < dynamicArray.Count; i++)
            {
                var row = dynamicArray[i];
                for (int j = 0; j < Math.Min(studentsPerGroup / 2, row.Count); j++)
                {
                    if (groupList.Count % studentsPerGroup == 0 && groupList.Count > 0)
                    {
                        groupNumber++;
                        Debug.Log("Incrementing group number: " + groupNumber);
                    }
                    GameObject student = row[0];
                    row.RemoveAt(0);
                    int index = Array.IndexOf(studentsHeads, student);
                    groupAssignment[index] = groupNumber;
                    groupList.Add(student);
                    if (groupList.Count % studentsPerGroup == 0 && groupList.Count > 0)
                    {
                        break;
                    }
                }
            }
        }

        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);

        for (int i = 0; i < studentsHeads.Length; i++)
        {
            Debug.Log($"Student {studentsHeads[i].name} assigned to group {groupAssignment[i]}");
        }
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
