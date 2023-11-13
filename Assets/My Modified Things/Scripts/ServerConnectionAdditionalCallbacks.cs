using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class ServerConnectionAdditionalCallbacks : PhotonPun.MonoBehaviourPunCallbacks
{

    // anything I want to have run automatically when joined room
    public override void OnJoinedRoom() {

        var newProperty = new ExitGames.Client.Photon.Hashtable { { "mainObjectCurrentModelName", "Model2" } };
        PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newProperty);

    }


    // in the future, we can automatically join a room if one exists
    public override void OnJoinedLobby() {

    }
}
