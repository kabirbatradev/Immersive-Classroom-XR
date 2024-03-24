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
        bool wallObjectsExist = null != GameObject.FindWithTag("CeilingMesh");

        // OVRCameraRig ovrCameraRig = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>();
        // var centerCamera = ovrCameraRig.centerEyeAnchor.GetComponent<Camera>();
        if (wallObjectsExist) {
            // show passthrough anyway if walls and ceiling have not been lowered at all
            if (StreamTheaterModeData.Instance.wallLoweredPercentage == 0
                    && StreamTheaterModeData.Instance.ceilingRemovedPercentage == 0) {
                
                centerCamera.clearFlags = CameraClearFlags.SolidColor;
            }
            else {
                // show the skybox if the walls/ceiling have been lowered
                centerCamera.clearFlags = CameraClearFlags.Skybox;
            }

            
        }
        // fallback for when wall objects don't exist
        else {
            // passthrough
            centerCamera.clearFlags = CameraClearFlags.SolidColor;
        }


    }
}
