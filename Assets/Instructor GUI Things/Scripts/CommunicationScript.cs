using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class CommunicationScript : MonoBehaviour
{
    public Camera mainCamera;
    private Vector3 CamPos;
    private Vector3 HitPos;

    void Update()
    {
        CamPos = mainCamera.transform.position;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            HitPos = hit.point;
            SetVariableOnServer("HitPos", HitPos);
        }

        SetVariableOnServer("CamPos", CamPos);
    }

    private void SetVariableOnServer(string key, object value)
    {
        Hashtable newTableEntry = new Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newTableEntry);
    }

    private object GetVariableOnServer(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }

    private bool VariableExistsOnServer(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }
}
