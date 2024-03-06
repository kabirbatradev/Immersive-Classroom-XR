using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPrefabScript : MonoBehaviour
{
    [SerializeField]
    private GameObject testPrefab;

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
    }
}
