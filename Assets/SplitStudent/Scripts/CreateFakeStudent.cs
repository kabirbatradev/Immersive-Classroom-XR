using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFakeStudent : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
                marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                marker.transform.position = new Vector3(i, 0, j);
                marker.AddComponent<Marker>();
                marker.GetComponent<Marker>().row = i;
                marker.GetComponent<Marker>().column = j;
                marker.tag = "Marker";
            }
        }

        // hardcode the group for each marker
        // [0,0], [0,1], [1,0], [1,1] -> group 1, etc
        GameObject[] markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject marker in markers)
        {
            Marker markerComponent = marker.GetComponent<Marker>();
            int groupNumber = (markerComponent.row / 2) * 3 + (markerComponent.column / 2) + 1;
            markerComponent.group = groupNumber;
        }

        // create fake student at some random location around each marker
        foreach (GameObject marker in markers)
        {
            GameObject fakeStudent = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            fakeStudent.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            fakeStudent.transform.position = new Vector3(marker.transform.position.x + Random.Range(-0.25f, 0.25f), 0, marker.transform.position.z + Random.Range(-0.5f, 0.5f));
            fakeStudent.AddComponent<FakeStudent>();
            fakeStudent.GetComponent<FakeStudent>().group = 1;
            fakeStudent.tag = "FakeStudent";
            fakeStudent.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
