using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class CloudFunctions : MonoBehaviour
{
    // void Start()
    // {
        
    // }

    // void Update()
    // {
        
    // }



    // player group number stuff
    public static int GetCurrentGroupNumber() {
        PhotonRealtime.Player localPlayer = PhotonNetwork.LocalPlayer;
        return GetPlayerGroupNumber(localPlayer);

    }

    public static int GetPlayerGroupNumber(PhotonRealtime.Player player) {
        int defaultGroupNumber = 1;

        ExitGames.Client.Photon.Hashtable PlayerProperties = player.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int groupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : defaultGroupNumber;

        return groupNumber;
    }


    // room custom properties
    public static bool RoomHasCustomProperty(string key) {
        if (PhotonNetwork.CurrentRoom == null) return false;
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }

    public static object GetRoomCustomProperty(string key) {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }


    public static void SetRoomCustomProperty(string key, object value) {
        var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomProperty);
        // also set locally for faster updates
        PhotonNetwork.CurrentRoom.CustomProperties[key] = value;
    }


    // object group number
    public static void SetPhotonObjectGroupNumber(GameObject photonObject, int groupNumber) {
        string key = "groupNum" + photonObject.GetComponent<PhotonView>().ViewID;
        int value = groupNumber;
        SetRoomCustomProperty(key, value);
    }

    public static bool PhotonObjectHasGroupNumber(GameObject photonObject) {
        string key = "groupNum" + photonObject.GetPhotonView().ViewID;
        return RoomHasCustomProperty(key);
    }

    public static int GetPhotonObjectGroupNumber(GameObject photonObject) {
        string key = "groupNum" + photonObject.GetPhotonView().ViewID;
        return (int)GetRoomCustomProperty(key);
    }


    public static bool HasJoinedPhotonRoom() {
        return PhotonNetwork.CurrentRoom == null;
    }


}
