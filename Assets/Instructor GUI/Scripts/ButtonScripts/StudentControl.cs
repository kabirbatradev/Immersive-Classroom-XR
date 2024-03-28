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

        // Assuming 6 rows, pre-define the maximum number of rows
        int maxRow = 5; // 0-based indexing, so row 6 is index 5

        // Sort students into a list based on their row and column for easier processing
        List<(GameObject student, int row, int col)> sortedStudents = new List<(GameObject student, int row, int col)>();

        foreach (GameObject student in studentsHeads)
        {
            int row = findRow(student);
            int col = findCol(student);
            sortedStudents.Add((student, row, col));
        }

        // Sort students by row and then by column to process in order
        sortedStudents.Sort((a, b) => a.row == b.row ? a.col.CompareTo(b.col) : a.row.CompareTo(b.row));

        int groupNumber = 1; // Start with group 1

        // We'll group students by taking two from one row and two from the next row
        for (int row = 0; row < maxRow; row += 2) // Iterate through rows in pairs
        {
            var currentRowStudents = sortedStudents.Where(s => s.row == row).ToList();
            var nextRowStudents = sortedStudents.Where(s => s.row == row + 1).ToList();

            int pairsInCurrentRow = currentRowStudents.Count / 2;
            int pairsInNextRow = nextRowStudents.Count / 2;
            int maxPairs = Math.Min(pairsInCurrentRow, pairsInNextRow);

            for (int i = 0; i < maxPairs * 2; i++)
            {
                if (i < currentRowStudents.Count)
                {
                    int studentIndex = Array.IndexOf(studentsHeads, currentRowStudents[i].student);
                    groupAssignment[studentIndex] = groupNumber + (i / 2);
                }

                if (i < nextRowStudents.Count)
                {
                    int studentIndex = Array.IndexOf(studentsHeads, nextRowStudents[i].student);
                    groupAssignment[studentIndex] = groupNumber + (i / 2);
                }
            }

            // Adjust groupNumber for next set of rows
            groupNumber += maxPairs;
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
