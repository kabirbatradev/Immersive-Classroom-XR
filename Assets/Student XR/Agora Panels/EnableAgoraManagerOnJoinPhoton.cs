using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAgoraManagerOnJoinPhoton : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {

        // if photon has loaded
        if (CloudFunctions.HasJoinedPhotonRoom()) {
            // enable the agora manager component
            GetComponent<AgoraManager>().enabled = true;
            
            // disable this script
            enabled = false;
        }

    }
}
