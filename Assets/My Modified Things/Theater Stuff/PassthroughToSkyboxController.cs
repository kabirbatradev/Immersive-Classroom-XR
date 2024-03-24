using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class PassthroughToSkyboxController : MonoBehaviour
{
    // private OVRCameraRig ovrCameraRig;
    private Camera centerCamera;
    void Start()
    {
        OVRCameraRig ovrCameraRig = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        centerCamera = ovrCameraRig.centerEyeAnchor.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        

        // if a ceiling object exists, then show Skybox; otherwise show passthrough
        bool exists = null != GameObject.FindWithTag("CeilingMesh");

        // OVRCameraRig ovrCameraRig = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        // var centerCamera = ovrCameraRig.centerEyeAnchor.GetComponent<Camera>();
        if (exists) {
            // image skybox
            centerCamera.clearFlags = CameraClearFlags.Skybox;
        }
        else {
            // passthrough
            centerCamera.clearFlags = CameraClearFlags.SolidColor;
        }


    }
}
