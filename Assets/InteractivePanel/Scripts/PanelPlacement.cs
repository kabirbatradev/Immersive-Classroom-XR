using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPlacement : MonoBehaviour
{
    private Vector3 _relativePos = new Vector3(0, -0.2f, 0);
    void Update()
    {
        transform.position = Camera.main.transform.position + _relativePos;
        Vector3 cameraRotationEuler = Camera.main.transform.rotation.eulerAngles;
        Vector3 newRotationEuler = new Vector3(0, cameraRotationEuler.y, 0);
        transform.rotation = Quaternion.Euler(newRotationEuler);
    }
}
