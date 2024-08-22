using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class CloudFunctions : MonoBehaviour
{

    // no need for instance because all functions are static

    // public CloudFunctions Instance;
    // private void Awake() {
    //     if (Instance == null)
    //         Instance = this;
    //     else
    //         Destroy(this);
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
        return PhotonNetwork.CurrentRoom != null;
    }

    public static int GetPlayerPresetGroupNumber(PhotonRealtime.Player player) {
        bool groupNumberExists = player.CustomProperties.ContainsKey("groupNumberPreset");
        int groupNumber = groupNumberExists ? (int)player.CustomProperties["groupNumberPreset"] : 999;
        return groupNumber;
    }
    public static void SetPlayerPresetGroupNumber(PhotonRealtime.Player player, int presetGroupNumber) {
        string key = "groupNumberPreset";
        int value = presetGroupNumber;
        var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
        // update on server
        player.SetCustomProperties(newCustomProperty);
        // update locally because server will update local cached hashmap with delay
        player.CustomProperties[key] = value;
    }

}
