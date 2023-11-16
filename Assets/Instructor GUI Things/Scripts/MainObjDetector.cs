using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class MainObjDetector : MonoBehaviour
{

    void Update()
    {
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        string currentActiveObject = (string)GetRoomCustomProperty("mainObjectCurrentModelName");
        foreach (Transform child in mainObjectContainer.transform)
        {
            GameObject potentialModel = child.gameObject;
            potentialModel.SetActive(potentialModel.name == currentActiveObject);
        }
    }
    private object GetRoomCustomProperty(string key)
    {
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];
    }
}