using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitStudent : MonoBehaviour
{
    public GameObject[] markers;
    public GameObject[] students;

    // ------------ Button Functions ------------
    public void SplitIndividual()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        students = GameObject.FindGameObjectsWithTag("FakeStudent");
        int group = 1;
        foreach (GameObject student in students)
        {
            student.GetComponent<FakeStudent>().group = group;
            group++;
        }
        setStudentColor(36);
    }

    public void SplitRow()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        students = GameObject.FindGameObjectsWithTag("FakeStudent");
        foreach (GameObject student in students)
        {
            int row = findRow(student);
            student.GetComponent<FakeStudent>().group = row + 1;
        }
        setStudentColor(6);
    }

    public void SplitDoubleRow()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        students = GameObject.FindGameObjectsWithTag("FakeStudent");
        foreach (GameObject student in students)
        {
            int row = findRow(student);
            student.GetComponent<FakeStudent>().group = (row / 2) + 1;
        }
        setStudentColor(3);
    }

    public void SplitFour()
    {
        markers = GameObject.FindGameObjectsWithTag("Marker");
        students = GameObject.FindGameObjectsWithTag("FakeStudent");
        foreach (GameObject student in students)
        {
            int group = findFourGroup(student);
            student.GetComponent<FakeStudent>().group = group;
        }
        setStudentColor(9);
    }
    // ------------ Button Functions ------------



    // ------------ Helper Functions ------------
    public int findRow(GameObject student)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(student.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetRow();
    }

    public int findCol(GameObject student)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(student.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetColumn();
    }

    public int findFourGroup(GameObject student)
    {
        float minDist = Mathf.Infinity;
        GameObject closestMarker = null;
        foreach (GameObject marker in markers)
        {
            float dist = Vector3.Distance(student.transform.position, marker.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestMarker = marker;
            }
        }
        return closestMarker.GetComponent<Marker>().GetGroup();
    }
    // ------------ Helper Functions ------------



    // ------------ Debug Functions ------------
    public void setStudentColor(int totalGroup)
    {
        Color[] colors = new Color[(int)totalGroup];
        for (int i = 0; i < totalGroup; i++)
        {
            colors[i] = Random.ColorHSV();
        }

        foreach (GameObject student in students)
        {
            int group = student.GetComponent<FakeStudent>().group;
            student.GetComponent<Renderer>().material.color = colors[group - 1];
        }
    }
    // ------------ Debug Functions ------------
}
