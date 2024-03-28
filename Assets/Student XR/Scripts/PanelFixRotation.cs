using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PanelFixRotation : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // LateUpdate is called every frame, if the Behaviour is enabled. It is called after all Update functions have been called.
    private void LateUpdate() {
        transform.rotation = transform.rotation.ConstrainYaw();
    }

}
