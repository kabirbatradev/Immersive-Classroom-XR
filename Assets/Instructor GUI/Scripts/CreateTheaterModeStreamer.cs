using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class CreateTheaterModeStreamer : PhotonPun.MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom() {

        // GameObject streamObject = GameObject.Find("StreamTheaterModeData");
        GameObject streamObject = GameObject.FindWithTag("StreamTheaterModeData");

        if (streamObject == null)
            Photon.Pun.PhotonNetwork.Instantiate("StreamTheaterModeData", Vector3.zero, Quaternion.identity);

        // make sure the owner of the object is the instructor
        streamObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);


    }
}
