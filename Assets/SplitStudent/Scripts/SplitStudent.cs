using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    // Used to split students into squares of 4
    public void SplitFour()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        studentsHeads = GameObject.FindGameObjectsWithTag("FakeStudent");

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

        // Sort each row based on their z position
        foreach (var row in rowDictionary)
        {
            row.Value.Sort((x, y) => x.transform.position.z.CompareTo(y.transform.position.z));
            dynamicArray.Add(row.Value);
        }

        List<GameObject> groupList = new List<GameObject>();
        int groupNumber = 1;
        int studentsPerGroup = 4;

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
                    }
                    GameObject student = row[0];
                    row.RemoveAt(0);
                    student.GetComponent<FakeStudent>().group = groupNumber;
                    groupList.Add(student);
                    if (groupList.Count % studentsPerGroup == 0 && groupList.Count > 0)
                    {
                        break;
                    }
                }
            }
        }

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
        for (int i = 0; i < studentsHeads.Length; i++)
        {
            int group = studentsHeads[i].GetComponent<FakeStudent>().group;
            // color by rgb with group where r g and b is group number * 10
            studentsHeads[i].GetComponent<Renderer>().material.color = new Color(group * 25 / 255f, group * 25 / 255f, group * 25 / 255f);
        }
    }
    // ------------ Helper Functions ------------
}
