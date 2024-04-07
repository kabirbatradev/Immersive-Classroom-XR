using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using RTG;

// namespace RTG
// {
public class RuntimeGizmo : MonoBehaviour
{
    private ObjectTransformGizmo objectRotateGizmo;
    private ObjectTransformGizmo objectScaleGizmo;
    private ObjectTransformGizmo objectMoveGizmo;
    private ObjectTransformGizmo laserMoveGizmo;
    private GameObject targetObject;
    public bool isGizmoActive = false;
    private bool forceCenter = false;
    // 0 -> Rotate
    // 1 -> Scale
    // 2 -> Move
    private int gizmoOption = 0;
    public GameObject laserStartPoint;

    private void Start()
    {
        objectRotateGizmo = RTGizmosEngine.Get.CreateObjectRotationGizmo();
        objectScaleGizmo = RTGizmosEngine.Get.CreateObjectScaleGizmo();
        objectMoveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
        laserMoveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            gizmoOption = 2;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gizmoOption = 0;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gizmoOption = 1;
        }

        if (!isGizmoActive)
        {
            objectRotateGizmo.Gizmo.SetEnabled(false);
            objectScaleGizmo.Gizmo.SetEnabled(false);
            objectMoveGizmo.Gizmo.SetEnabled(false);
            laserMoveGizmo.Gizmo.SetEnabled(false);
            return;
        }

        if (CamRotate.Instance.currentTarget.gameObject == null)
        {
            return;
        }
        targetObject = CamRotate.Instance.currentTarget.gameObject;

        // Update the gizmo only if it is active and the target object is found
        if (isGizmoActive && targetObject != null)
        {
            // Object
            if (gizmoOption == 0)
            {
                objectRotateGizmo.Gizmo.SetEnabled(true);
                objectScaleGizmo.Gizmo.SetEnabled(false);
                objectMoveGizmo.Gizmo.SetEnabled(false);
                objectRotateGizmo.SetTargetObject(targetObject);
            }
            else if (gizmoOption == 1)
            {
                objectScaleGizmo.Gizmo.SetEnabled(true);
                objectRotateGizmo.Gizmo.SetEnabled(false);
                objectMoveGizmo.Gizmo.SetEnabled(false);
                objectScaleGizmo.SetTargetObject(targetObject);
            }
            else if (gizmoOption == 2)
            {
                objectMoveGizmo.Gizmo.SetEnabled(true);
                objectRotateGizmo.Gizmo.SetEnabled(false);
                objectScaleGizmo.Gizmo.SetEnabled(false);
                objectMoveGizmo.SetTargetObject(targetObject);
            }
            // Laser
            laserMoveGizmo.Gizmo.SetEnabled(true);
            laserMoveGizmo.SetTargetObject(laserStartPoint);
        }
        else
        {
            objectRotateGizmo.Gizmo.SetEnabled(false);
            objectScaleGizmo.Gizmo.SetEnabled(false);
            laserMoveGizmo.Gizmo.SetEnabled(false);
        }

        if (targetObject != null && forceCenter)
        {
            targetObject.transform.position = new Vector3(0, 0, 0);
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

    public void ToggleGizmo(bool status)
    {
        isGizmoActive = status;
    }
}
