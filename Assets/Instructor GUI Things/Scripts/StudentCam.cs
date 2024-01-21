using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class StudentCam : MonoBehaviour
{
    public GameObject studentCam;

    void Start()
    {

    }

    void Update()
    {
        GameObject studentHead = GameObject.Find("MyPhotonUserHeadTracker(Clone)");

        if (studentHead != null)
        {
            studentCam.transform.position = studentHead.transform.position;
            studentCam.transform.rotation = studentHead.transform.rotation;
        }
    }

    private object GetRoomCustomProperty(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    private bool RoomHasCustomProperty(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }
}
