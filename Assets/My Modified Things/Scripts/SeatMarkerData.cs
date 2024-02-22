using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public struct MarkerDataStruct {
    public int row;
    public int column;
    public int totalColumnsInThisRow;
}

public class MarkerData : MonoBehaviour
{

    private PhotonView photonView;

    private MarkerDataStruct thisMarkerData;
    private bool markerDataSet = false;

    void Start() {
        photonView = gameObject.GetPhotonView();
    }

    // void Update() {
        
    // }

    private bool SetThisMarkerDataCache() {
        if (photonView == null) {
            return false;
        }

        string key = "marker" + photonView.ViewID;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key)) {
            return false;
        }

        thisMarkerData = (MarkerDataStruct)PhotonNetwork.CurrentRoom.CustomProperties[key];

        return true;
    }


    public int GetRow() {
        if (markerDataSet == false) {
            SetThisMarkerDataCache();
            markerDataSet = true;
        }

        return thisMarkerData.row;
    }

    public int GetColumn() {
        if (markerDataSet == false) {
            SetThisMarkerDataCache();
            markerDataSet = true;
        }

        return thisMarkerData.column;
    }

    public int GetTotalColumnsInThisRow() {
        if (markerDataSet == false) {
            SetThisMarkerDataCache();
            markerDataSet = true;
        }

        return thisMarkerData.totalColumnsInThisRow;
    }




}
