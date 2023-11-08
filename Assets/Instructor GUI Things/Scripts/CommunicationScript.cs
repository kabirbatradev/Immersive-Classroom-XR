using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class CommunicationScript : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    private int num = 0;

    // Update is called once per frame
    void Update()
    {   

        // ignore the fact that this keeps giving errors until we have actually connected to the room
        SetVariableOnServer("testNumber", num);
        num++;
    }


    private void SetVariableOnServer(string key, object value) {
        var newTableEntry = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newTableEntry);
    }

    private object GetVariableOnServer(string key) {
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    private bool VariableExistsOnServer(string key) {
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }
} 
