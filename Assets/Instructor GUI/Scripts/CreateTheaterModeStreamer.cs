using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class CreateTheaterModeStreamer : PhotonPun.MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom() {

        // GameObject[] streamObjects = GameObject.FindGameObjectsWithTag("StreamTheaterModeData");
        // Debug.Log("OnJoinedRoom debug log stream objects length: " + streamObjects.Length);

        // GameObject streamObject = GameObject.Find("StreamTheaterModeData");
        // it seems to have failed to find this object
        // GameObject streamObject = GameObject.FindWithTag("StreamTheaterModeData");

        // no need to even read the value, we know its true; what matters is that it exists
        bool alreadyInstantiated = InstructorCloudFunctions.Instance.RoomHasCustomProperty("StreamTheaterModeDataInstantiated");
        Debug.Log("OnJoinedRoom StreamTheaterModeDataInstantiated: " + alreadyInstantiated);
        if (!alreadyInstantiated) {
            Photon.Pun.PhotonNetwork.Instantiate("StreamTheaterModeData", Vector3.zero, Quaternion.identity);
            alreadyInstantiated = true;
            InstructorCloudFunctions.Instance.SetRoomCustomProperty("StreamTheaterModeDataInstantiated", alreadyInstantiated);
        }

        // make sure the owner of the object is the instructor
        // we actually cant do this yet because it might not have been created yet
        // streamObject.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);


    }
}
