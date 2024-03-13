using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnableOVRSceneManager : MonoBehaviour
{


    [SerializeField]
    private GameObject OVRSceneManagerObj;

    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {   
        bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.X);
        if (buttonPressed) {
            // flip the active state of this object
            OVRSceneManagerObj.SetActive(!OVRSceneManagerObj.activeSelf);
        }
        
    }
}
