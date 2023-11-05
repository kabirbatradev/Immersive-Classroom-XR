using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PhotonUserHeadTrackerCommunication : MonoBehaviour, IPunObservable
{

    private PhotonView photonView;

    // get the photon view of the gameobject
    private void Start() {
        photonView = GetComponent<PhotonView>();
        // photonView.isMine tells us if this object was instantiated locally or not
        // use this to not render the cube if it is of the self

        if (photonView.IsMine) {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(false);
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

            Transform localHead = UserHeadPositionTrackerManager.Instance.localHeadTransform;

            stream.SendNext(localHead.position);
            stream.SendNext(localHead.eulerAngles);
        }

        // if reading, this object must have been instantiated elsewhere (it is someone elses head)
        else {

            // by recieving data on someone elses head, we can move around this current prefab itself
                // if this prefab has a child object with an actual mesh, then we will see that mesh correspond to the head
                // of another user
            
            gameObject.transform.position = (Vector3)stream.ReceiveNext();
            gameObject.transform.eulerAngles = (Vector3)stream.ReceiveNext();
        }

    }
}
