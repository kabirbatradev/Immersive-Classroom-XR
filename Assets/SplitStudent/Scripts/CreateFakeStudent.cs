using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFakeStudent : MonoBehaviour
{
    [SerializeField]
    private float dropoutRate = 0.2f;  // 20% chance to not create a student at a marker

    void Start()
    {
        // Create grid of markers
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

        // Assign group numbers to markers based on their positions
        GameObject[] markers = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject marker in markers)
        {
            Marker markerComponent = marker.GetComponent<Marker>();
            int groupNumber = (markerComponent.row / 2) * 3 + (markerComponent.column / 2) + 1;
            markerComponent.group = groupNumber;
        }

        // Create fake students at markers with a chance of dropout
        foreach (GameObject marker in markers)
        {
            // Check if the student at this marker should be "dropped out"
            if (Random.value > dropoutRate)  // Random.value returns a float between 0.0 and 1.0
            {
                GameObject fakeStudent = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fakeStudent.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                fakeStudent.transform.position = new Vector3(marker.transform.position.x + Random.Range(-0.2f, 0.2f), 0, marker.transform.position.z + Random.Range(-0.5f, 0.5f));
                fakeStudent.AddComponent<FakeStudent>();
                fakeStudent.GetComponent<FakeStudent>().group = marker.GetComponent<Marker>().group;
                fakeStudent.tag = "FakeStudent";
                fakeStudent.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}
