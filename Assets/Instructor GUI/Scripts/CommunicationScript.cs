using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class CommunicationScript : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject mainCameraObj;
    public GameObject laserStartPoint;
    public bool laserFromCamera = true;
    private Vector3 StartPos;
    private Vector3 HitPos;
    public GameObject CurObj;
    private Quaternion ObjRot;
    private Vector3 ObjPos;
    private Vector3 ObjScale;
    private bool IsShooting = false;
    private static bool serverLaser = true;

    void FixedUpdate()
    {
        // current target or current object is the main object holder
        // if it doesnt exist, then there is no data to send about where the laser is etc
        if (CamRotate.Instance.currentTarget == null) return;
        CurObj = CamRotate.Instance.currentTarget.gameObject;

        if (Input.GetKeyDown(KeyCode.C))
        {
            laserFromCamera = !laserFromCamera;
        }

        if (laserFromCamera == false)
        {
            StartPos = laserStartPoint.transform.position;
        }
        else
        {
            StartPos = mainCameraObj.transform.position;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // CamRotate does this for us already, and its saved in "currentTarget"
        ObjRot = CurObj.transform.rotation;
        ObjScale = CurObj.transform.localScale;
        ObjPos = CurObj.transform.position;

        // CurObj.transform.position = new Vector3(0, 0, 0);

        if (Physics.Raycast(ray, out hit) && Input.GetMouseButton(0) && serverLaser)
        {
            HitPos = hit.point;
            IsShooting = true;
            SetVariableOnServer("HitPosition", HitPos - CurObj.transform.position);
            SetVariableOnServer("IsShooting", IsShooting);
        }
        else
        {
            IsShooting = false;
            SetVariableOnServer("IsShooting", IsShooting);
        }

        SetVariableOnServer("CameraPosition", StartPos);
        SetVariableOnServer("ObjectRotation", ObjRot);
        SetVariableOnServer("ObjectLocalPosition", ObjPos / 2);
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

    private GameObject FindTargetByName(string targetName)
    {
        // find it in the main object container
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        if (mainObjectContainer != null)
        {
            foreach (Transform child in mainObjectContainer.transform)
            {
                if (child.gameObject.name == targetName)
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    public static void ToggleServerLaser(bool status)
    {
        serverLaser = status;
    }
}
