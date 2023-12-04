using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MainObjDetector : MonoBehaviour
{
    public float updateFrequency = 1f; // once per second

    private void Start()
    {
        StartCoroutine(CheckServerState());
    }

    private IEnumerator CheckServerState()
    {
        while (true)
        {
            GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
            string currentActiveObject = (string)GetRoomCustomProperty("mainObjectCurrentModelName");

            foreach (Transform child in mainObjectContainer.transform)
            {
                GameObject potentialModel = child.gameObject;
                potentialModel.SetActive(potentialModel.name == currentActiveObject);
            }

            yield return new WaitForSeconds(1f / updateFrequency);
        }
    }

    private object GetRoomCustomProperty(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }
}
