using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class RPCFunctions : MonoBehaviourPun
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetKeyDown("space")) {
            Debug.Log("calling RPC function");
            photonView.RPC("TestRPC", RpcTarget.All, "I am calling the rpc; hello!");
        }
    }

    [PunRPC]
    void TestRPC(string logText)
    {
        SampleController.Instance.Log("log text: " + logText);
    }


}
