using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPrefabScript : MonoBehaviour
{
    [SerializeField]
    private GameObject testPrefab;
    [SerializeField]
    private GameObject testPrefab2;

    [SerializeField]
    private Transform rightHandTransform, leftHandTransform;

    // Update is called once per frame
    void Update()
    {
        bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.X);
        if (buttonPressed) {
            Vector3 position = leftHandTransform.position;
            Quaternion rotation = Quaternion.identity;
            Instantiate(testPrefab, position, rotation);
        }



        if (OVRInput.GetDown(OVRInput.RawButton.Y)) {
            Vector3 position = leftHandTransform.position;
            Quaternion rotation = Quaternion.identity;
            Instantiate(testPrefab2, position, rotation);
        }
    }
}
