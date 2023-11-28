using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class CommunicationScript : MonoBehaviour
{
    public Camera mainCamera;
    private Vector3 CamPos;
    private Vector3 HitPos;
    public GameObject CurObj;
    private Quaternion ObjRot;
    private Vector3 ObjPos;
    private Vector3 ObjScale;
    private bool IsShooting = false;

    void Update()
    {
        if (CamRotate.Instance.currentTarget == null) return;
        CurObj = CamRotate.Instance.currentTarget.gameObject;


        CamPos = mainCamera.transform.position;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        ObjRot = CurObj.transform.rotation;
        ObjPos = CurObj.transform.position;
        ObjScale = CurObj.transform.localScale;

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0))
        {
            HitPos = hit.point;
            IsShooting = true;
            SetVariableOnServer("HitPosition", HitPos);
            SetVariableOnServer("IsShooting", IsShooting);
        }
        else
        {
            IsShooting = false;
            SetVariableOnServer("IsShooting", IsShooting);
        }

        SetVariableOnServer("CameraPosition", CamPos);
        SetVariableOnServer("ObjectRotation", ObjRot);
        SetVariableOnServer("ObjectPosition", ObjPos);
        SetVariableOnServer("ObjectScale", ObjScale);
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
