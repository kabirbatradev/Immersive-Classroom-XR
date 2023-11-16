using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;

public class CommunicationScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject CurObj;
    public float updateFrequency = 1f; // Once per second

    private Vector3 camPos;
    private Vector3 hitPos;
    private Quaternion objRot;
    private Vector3 objPos;
    private Vector3 objScale;
    private bool IsShooting = false;

    private void Start()
    {
        StartCoroutine(UpdateServerState());
    }

    void Update()
    {
        camPos = mainCamera.transform.position;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            hitPos = hit.point;
            IsShooting = true;
        }
        else
        {
            IsShooting = false;
        }
    }

    private IEnumerator UpdateServerState()
    {
        while (true)
        {
            objRot = CurObj.transform.rotation;
            objPos = CurObj.transform.position;
            objScale = CurObj.transform.localScale;

            SetVariableOnServer("CameraPosition", camPos);
            SetVariableOnServer("ObjectRotation", objRot);
            SetVariableOnServer("ObjectPosition", objPos);
            SetVariableOnServer("ObjectScale", objScale);

            if (IsShooting)
            {
                SetVariableOnServer("HitPosition", hitPos);
            }
            SetVariableOnServer("IsShooting", IsShooting);

            yield return new WaitForSeconds(1f / updateFrequency);
        }
    }

    private void SetVariableOnServer(string key, object value)
    {
        ExitGames.Client.Photon.Hashtable newTableEntry = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newTableEntry);
    }
}
