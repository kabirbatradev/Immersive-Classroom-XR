using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace RTG
{
    public class RuntimeGizmo : MonoBehaviour
    {
        private ObjectTransformGizmo objectRotateGizmo;
        private ObjectTransformGizmo objectScaleGizmo;
        private ObjectTransformGizmo laserMoveGizmo;
        private GameObject targetObject;
        private static bool isGizmoActive = false;
        private bool forceCenter = true;
        public GameObject laserStartPoint;

        private void Start()
        {
            objectRotateGizmo = RTGizmosEngine.Get.CreateObjectRotationGizmo();
            objectScaleGizmo = RTGizmosEngine.Get.CreateObjectScaleGizmo();
            laserMoveGizmo = RTGizmosEngine.Get.CreateObjectMoveGizmo();
        }

        private void Update()
        {
            // Toggle the gizmo active state when 'T' key is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                isGizmoActive = !isGizmoActive;
            }

            targetObject = CamRotate.Instance.currentTarget.gameObject;

            // Update the gizmo only if it is active and the target object is found
            if (isGizmoActive && targetObject != null)
            {
                // Object
                objectRotateGizmo.Gizmo.SetEnabled(true);
                objectRotateGizmo.SetTargetObject(targetObject);
                objectScaleGizmo.Gizmo.SetEnabled(true);
                objectScaleGizmo.SetTargetObject(targetObject);
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

        public static void ToggleGizmo(bool status)
        {
            {
                isGizmoActive = status;
            }
        }
    }
}
