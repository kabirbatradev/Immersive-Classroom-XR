using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class UserHeadPositionTrackerManager : PhotonPun.MonoBehaviourPunCallbacks
{

    // this class is a singleton so that the photon prefab for the head can get the head transform data from this script easily
    public static UserHeadPositionTrackerManager Instance;

    public Transform localHeadTransform; // set to center eye anchor

    private void Awake() {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    // this function is automatically called since our class extends PhotonPun.MonoBehaviourPunCallbacks
    public override void OnJoinedRoom() {

        // HeadPrefab exists in the resources folder so Photon should be able to access it
        Photon.Pun.PhotonNetwork.Instantiate("MyPhotonUserHeadTracker", Vector3.zero, Quaternion.identity);

        // GameObject t = Photon.Pun.PhotonNetwork.Instantiate("MyPhotonUserHeadTracker", Vector3.zero, Quaternion.identity);
        // SampleController.Instance.Log(t.transform.GetChild(1).gameObject.name + t.transform.GetChild(1).gameObject.activeSelf);
        // Debug.Log(t.transform.GetChild(1).gameObject.name + t.transform.GetChild(1).gameObject.activeSelf);
    }
}
