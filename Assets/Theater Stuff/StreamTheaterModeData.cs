using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class StreamTheaterModeData : MonoBehaviour, IPunObservable
{


    // singleton structure for easy access
    public static StreamTheaterModeData Instance;
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }



    private PhotonView photonView;


    // variables to manage how much the ceiling has opened and how much the walls have dropped
    // these variables are updated by button presses on the instructor side, or they are set by OnPhotonSerializeView
    public float wallLoweredPercentage = 0.5f;
    public float ceilingRemovedPercentage = 0.5f;

    // ceilingVisible will be a custom server property since it doesn't have to be streamed constantly
    public bool ceilingVisible = true;




    // public GameObject head;

    // get the photon view of the gameobject
    private void Start() {
        photonView = GetComponent<PhotonView>();
        // photonView.isMine tells us if this object was instantiated locally or not
        // use this to not render the cube if it is of the self

        // if (photonView.IsMine) {
        //     // foreach (Transform child in transform) {
        //     //     child.gameObject.SetActive(false);
        //     // }
        //     head.SetActive(false);
        // }
    }

    private void Update() {

        // if (photonView.IsMine) {

        //     Transform localHead = UserHeadPositionTrackerManager.Instance.localHeadTransform;

        //     gameObject.transform.position = localHead.position;
        //     gameObject.transform.eulerAngles = localHead.eulerAngles;

        // }

        if (photonView.IsMine) {
            // this is the instructor, so update the bool value

            string key = "ceilingVisible";
            bool value = ceilingVisible;

            var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
            // update on server
            PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomProperty);

            // update locally because server will update local cached hashmap with delay
            PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key] = value;
        }
        else {
            // this is a student, so read the bool value
            string key = "ceilingVisible";

            // if the custom property doesnt exist, then assume true
            if (PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key)) {
                ceilingVisible = (bool)PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];
            }
            else {
                ceilingVisible = true;
            }

        }



    }

    // this function is called every time this object tries to update itself
    // if this object was instantiated locally (it is the local head), then it only sends data
    // otherwise, this object must have been instantiated elsewhere (it is someone elses head), so it recieves data
    // streaming happens many times per second (very fast)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        // if writing, then this object was instantiated locally (it is the local head)
        if (stream.IsWriting) {

            // write the head data of the head transform located in the UserHeadPositionTrackerManager
                // this script has only 1 instance, and we passed in the "center eye anchor" aka the position of the head

            // Transform localHead = UserHeadPositionTrackerManager.Instance.localHeadTransform;

            // stream.SendNext(localHead.position);
            // stream.SendNext(localHead.eulerAngles);


            stream.SendNext(wallLoweredPercentage);
            stream.SendNext(ceilingRemovedPercentage);

        }

        // if reading, this object must have been instantiated elsewhere (it is someone elses head)
        else {

            // by recieving data on someone elses head, we can move around this current prefab itself
                // if this prefab has a child object with an actual mesh, then we will see that mesh correspond to the head
                // of another user
            
            // gameObject.transform.position = (Vector3)stream.ReceiveNext();
            // gameObject.transform.eulerAngles = (Vector3)stream.ReceiveNext();

            wallLoweredPercentage = (float)stream.ReceiveNext();
            ceilingRemovedPercentage = (float)stream.ReceiveNext();
        }

    }
}
