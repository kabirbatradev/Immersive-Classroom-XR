using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SplitStudent : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject[] studentsHeads;

    // ------------ Button Functions ------------
    // Used to split students into individual groups
    public void SplitIndividual()
    {
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
        colorByGroup(studentsHeads);
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

    public void SplitFour()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        studentsHeads = GameObject.FindGameObjectsWithTag("FakeStudent");

        if (studentsHeads.Length == 0)
        {
            Debug.Log("No students available!");
            return;
        }

        var rowDictionary = new Dictionary<int, List<GameObject>>();

        // Fill dictionary with students categorized by their row
        foreach (var student in studentsHeads)
        {
            int row = findRow(student);
            if (!rowDictionary.ContainsKey(row))
            {
                rowDictionary[row] = new List<GameObject>();
            }
            rowDictionary[row].Add(student);
        }

        // Sort students within each row by their z position
        foreach (var row in rowDictionary)
        {
            row.Value.Sort((x, y) => x.transform.position.z.CompareTo(y.transform.position.z));
        }

        var groups = new List<List<GameObject>>();
        List<int> keys = new List<int>(rowDictionary.Keys);
        keys.Sort(); // Ensure rows are processed in numerical order

        // Pair rows and group students
        int groupNumber = 1; // Start the group numbering from 1
        for (int i = 0; i < keys.Count - 1; i += 2) // increment by 2 to ensure pairs (1,2), (3,4)...
        {
            var row1 = rowDictionary[keys[i]];
            var row2 = rowDictionary[keys[i + 1]];

            // Continue grouping until both rows are exhausted or cannot fill another complete group
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
                    student.GetComponent<FakeStudent>().group = groupNumber;
                }
                groups.Add(group);
                groupNumber++; // Increment group number for each new group
            }
        }

        // Handle leftovers for each row pair separately to prevent mixing
        for (int i = 0; i < keys.Count; i++)
        {
            var leftoverRow = rowDictionary[keys[i]];
            while (leftoverRow.Count > 0)
            {
                var group = new List<GameObject>();
                int takeCount = Math.Min(4, leftoverRow.Count); // Try to form groups of up to 4 from leftovers
                for (int j = 0; j < takeCount; j++)
                {
                    group.Add(leftoverRow[0]);
                    leftoverRow.RemoveAt(0);
                }
                foreach (var student in group)
                {
                    student.GetComponent<FakeStudent>().group = groupNumber;
                }
                groups.Add(group);
                groupNumber++; // Increment group number for each new group even with leftovers
            }
        }

        // Optional: Update visualization or UI
        colorByGroup(studentsHeads);
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

    public void colorByGroup(GameObject[] studentsHeads)
    {
        Dictionary<int, Color> groupColors = new Dictionary<int, Color>();

        for (int i = 0; i < studentsHeads.Length; i++)
        {
            int group = studentsHeads[i].GetComponent<FakeStudent>().group;
            if (!groupColors.ContainsKey(group))
            {
                // Generate a random color for each new group using UnityEngine.Random
                groupColors[group] = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            }

            // Assign the color to the student based on their group
            studentsHeads[i].GetComponent<Renderer>().material.color = groupColors[group];
        }
    }

    // ------------ Helper Functions ------------
}
