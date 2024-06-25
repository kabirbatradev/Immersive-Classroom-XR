using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PanelMarkerData : MonoBehaviour
{
    private PhotonView photonView;
    void Start() {
        photonView = gameObject.GetPhotonView();
    }

    public int GetPanelMarkerNumber() {
        string id = "panelMarker" + photonView.ViewID;
        if (CloudFunctions.RoomHasCustomProperty(id)) {
            return (int)CloudFunctions.GetRoomCustomProperty(id);
        }
        return -1;
    }
}
