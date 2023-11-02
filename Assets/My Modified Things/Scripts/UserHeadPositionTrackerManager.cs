using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class UserHeadPositionTrackerManager : PhotonPun.MonoBehaviourPunCallbacks
{

    // this class is a singleton so that the photon prefab for the head can get the head transform data from this script easily
    public static UserHeadPositionTrackerManager Instance;

    public Transform localHeadTransform;

    private void Awake() {
        Instance = this;

        // if (Instance == null)
        // {
        //     Instance = this;
        // }
        // else
        // {
        //     Destroy(this);
        // }
    }


    // this function is automatically called since our class extends PhotonPun.MonoBehaviourPunCallbacks
    public override void OnJoinedRoom() {

        // HeadPrefab exists in the resources folder so Photon should be able to access it
        Photon.Pun.PhotonNetwork.Instantiate("MyPhotonUserHeadTracker", Vector3.zero, Quaternion.identity);

    }
}
