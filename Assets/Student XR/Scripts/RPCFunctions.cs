using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using PhotonPun = Photon.Pun;
using PhotonRealtime =   Photon.Realtime;

public class RPCFunctions : MonoBehaviourPun
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     if (OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetKeyDown("space")) {
    //         Debug.Log("calling RPC function");
    //         photonView.RPC("TestRPC", RpcTarget.All, "I am calling the rpc; hello!");
    //     }
    // }

    [PunRPC]
    void TestRPC(string logText)
    {
        SampleController.Instance.Log("log text: " + logText);
    }


    [PunRPC]
    void KickSelfOutOfRoom()
    {
        // first, make sure the player is not the instructor or an admin
        // assume the instructor does not have an instance of this RPC functions script (because they also do not have CloudFunctions script)
        int myGroupNumber = CloudFunctions.GetCurrentGroupNumber();

        // if not admin, then leave room
        if (myGroupNumber > 0) {

            // very similar to what the exit room button does
            // will kick student out of room, but they will still automatically rejoin (intentional)

            PhotonNetwork.LeaveRoom(); 
            GUIManager.Instance.ResetControlPanel();
        }

    }

    // all students will be kicked from the room and then will rejoin
    // this allows users to have anchors reshared with them, allowing for realignment
    void KickAllStudentsOutOfRoom() {
        photonView.RPC("KickSelfOutOfRoom", RpcTarget.Others);
    }


}
