using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestClearFlagsPassthrough : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.Y);
        if (buttonPressed) {

            OVRCameraRig ovrCameraRig = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
            var centerCamera = ovrCameraRig.centerEyeAnchor.GetComponent<Camera>();

            if (centerCamera.clearFlags == CameraClearFlags.SolidColor) {
                centerCamera.clearFlags = CameraClearFlags.Skybox;
            }
            else {
                centerCamera.clearFlags = CameraClearFlags.SolidColor;
            }

            // centerCamera.backgroundColor = Color.clear;
        }
    }
}
