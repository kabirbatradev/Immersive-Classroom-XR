using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject camera;
    public CamRotate camScript;
    public void ResetCamera()
    {
        camera.transform.position = new Vector3(0, 0, -1);
        camera.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void LockOnEnable()
    {
        camScript.lockOn = true;
    }

    public void LockOnDisable()
    {
        camScript.lockOn = false;
    }
}
