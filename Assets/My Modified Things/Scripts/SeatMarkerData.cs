using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// public struct MarkerDataStruct {
//     public int row;
//     public int column;
//     public int totalSeatsInThisRow;
// }

public class SeatMarkerData : MonoBehaviour
{

    private PhotonView photonView;

    // private MarkerDataStruct thisMarkerData;
    private int row;
    private int column;
    private int totalSeatsInThisRow;
    private bool markerDataSet = false;

    void Start() {
        photonView = gameObject.GetPhotonView();
    }

    // void Update() {
        
    // }

    // private bool SetThisMarkerDataCache() {
    //     if (photonView == null) {
    //         return false;
    //     }

    //     string key = "marker" + photonView.ViewID;
    //     if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key)) {
    //         return false;
    //     }

    //     thisMarkerData = (MarkerDataStruct)PhotonNetwork.CurrentRoom.CustomProperties[key];

    //     return true;
    // }
    private bool CacheMarkerData() {
        if (photonView == null) {
            return false;
        }

        string key = "marker" + photonView.ViewID;
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key)) {
            return false;
        }

        int[] thisMarkerData = (int[])PhotonNetwork.CurrentRoom.CustomProperties[key];
        row = thisMarkerData[0];
        column = thisMarkerData[1];
        totalSeatsInThisRow = thisMarkerData[2];

        return true;
    }


    public int GetRow() {
        if (markerDataSet == false) {
            markerDataSet = CacheMarkerData();
        }
        if (!markerDataSet) return -1;

        return row;
    }

    public int GetColumn() {
        if (markerDataSet == false) {
            markerDataSet = true;
            markerDataSet = CacheMarkerData();
        }
        if (!markerDataSet) return -1;

        return column;
    }

    public int GetTotalSeatsInThisRow() {
        if (markerDataSet == false) {
            markerDataSet = CacheMarkerData();
        }
        if (!markerDataSet) return -1;

        return totalSeatsInThisRow;
    }




}
