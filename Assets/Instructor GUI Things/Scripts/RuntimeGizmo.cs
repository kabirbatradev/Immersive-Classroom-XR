using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace RTG
{
    public class RuntimeGizmo : MonoBehaviour
    {
        private ObjectTransformGizmo objectTransformGizmo;
        private ObjectTransformGizmo laserTransformGizmo;
        private GameObject targetObject;
        private bool isGizmoActive = false;
        private bool forceCenter = true;
        public GameObject laserStartPoint;

        private void Start()
        {
            objectTransformGizmo = RTGizmosEngine.Get.CreateObjectUniversalGizmo();
            laserTransformGizmo = RTGizmosEngine.Get.CreateObjectUniversalGizmo();
        }

        private void Update()
        {
            // Toggle the gizmo active state when 'T' key is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                isGizmoActive = !isGizmoActive;
            }

            // Check if the targetObject has been assigned or if it needs to be found
            if (targetObject == null)
            {
                GameObject targetObjectParent = GameObject.FindWithTag("MainObjectContainer");
                string currentActiveObjectName = (string)GetRoomCustomProperty("mainObjectCurrentModelName");
                GameObject foundTarget = GameObject.Find(currentActiveObjectName);
                if (foundTarget != null)
                {
                    targetObject = foundTarget;
                }
            }

            // Update the gizmo only if it is active and the target object is found
            if (isGizmoActive && targetObject != null)
            {
                objectTransformGizmo.Gizmo.SetEnabled(true);
                objectTransformGizmo.SetTargetObject(targetObject);
                laserTransformGizmo.Gizmo.SetEnabled(true);
                laserTransformGizmo.SetTargetObject(laserStartPoint);
            }
            else
            {
                objectTransformGizmo.Gizmo.SetEnabled(false);
                laserTransformGizmo.Gizmo.SetEnabled(false);
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
    }
}
