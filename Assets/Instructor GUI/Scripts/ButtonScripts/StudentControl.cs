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

    public void SplitFour()
    {
        markers = GameObject.FindGameObjectsWithTag("SeatMarker");
        studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
        Debug.Log("Number of students: " + studentsHeads.Length);
        int[] groupAssignment = new int[studentsHeads.Length];
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

        // Sort each row based on their x position
        foreach (var row in rowDictionary)
        {
            row.Value.Sort((x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
        }

        var groups = new List<List<GameObject>>();
        List<int> keys = new List<int>(rowDictionary.Keys);
        keys.Sort(); // Ensure rows are processed in numerical order

        // Pair rows and group students
        int groupNumber = 1;
        for (int i = 0; i < keys.Count - 1; i += 2) // increment by 2 to ensure pairs (1,2), (3,4)...
        {
            var row1 = rowDictionary[keys[i]];
            var row2 = rowDictionary[keys[i + 1]];

            while (row1.Count > 0 && row2.Count > 0)
            {
                var group = new List<GameObject>();
                int count = Math.Min(2, row1.Count);
                for (int j = 0; j < count; j++)
                {
                    group.Add(row1[0]);
                    row1.RemoveAt(0);
                }

                count = Math.Min(2, row2.Count);
                for (int j = 0; j < count; j++)
                {
                    group.Add(row2[0]);
                    row2.RemoveAt(0);
                }

                foreach (var student in group)
                {
                    int index = Array.IndexOf(studentsHeads, student);
                    groupAssignment[index] = groupNumber;
                }
                groups.Add(group);
                groupNumber++;
            }

            // Handle any leftovers in each row pair right after processing them
            HandleLeftoverStudents(row1, ref groupNumber, groupAssignment, studentsHeads, groups);
            HandleLeftoverStudents(row2, ref groupNumber, groupAssignment, studentsHeads, groups);
        }

        // Handle any leftover rows if the number of rows is odd
        if (keys.Count % 2 != 0)
        {
            var leftoverRow = rowDictionary[keys[keys.Count - 1]];
            HandleLeftoverStudents(leftoverRow, ref groupNumber, groupAssignment, studentsHeads, groups);
        }

        InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(studentsHeads, groupAssignment);

        for (int i = 0; i < studentsHeads.Length; i++)
        {
            Debug.Log($"Student {studentsHeads[i].name} assigned to group {groupAssignment[i]}");
        }
    }

    private void HandleLeftoverStudents(List<GameObject> leftoverRow, ref int groupNumber, int[] groupAssignment, GameObject[] studentsHeads, List<List<GameObject>> groups)
    {
        while (leftoverRow.Count > 0)
        {
            var group = new List<GameObject>();
            int takeCount = Math.Min(4, leftoverRow.Count); // Try to form groups of up to 4
            for (int j = 0; j < takeCount; j++)
            {
                group.Add(leftoverRow[0]);
                leftoverRow.RemoveAt(0);
            }

            foreach (var student in group)
            {
                int index = Array.IndexOf(studentsHeads, student);
                groupAssignment[index] = groupNumber;
            }
            groups.Add(group);
            groupNumber++;
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
