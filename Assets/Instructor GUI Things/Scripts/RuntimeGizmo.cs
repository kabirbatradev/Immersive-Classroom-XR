using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTG
{
    public class RuntimeGizmo : MonoBehaviour
    {
        private ObjectTransformGizmo objectTransformGizmo;
        private GameObject targetObject;
        private bool isGizmoActive = false;

        private void Start()
        {
            objectTransformGizmo = RTGizmosEngine.Get.CreateObjectUniversalGizmo();
        }

        private void Update()
        {
            // Toggle the gizmo active state when 'T' key is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                isGizmoActive = !isGizmoActive;
                Debug.Log("Gizmo active: " + isGizmoActive);
            }

            // Check if the targetObject has been assigned or if it needs to be found
            if (targetObject == null)
            {
                targetObject = GameObject.FindWithTag("MainObjectContainer");
            }

            // Update the gizmo only if it is active and the target object is found
            if (isGizmoActive && targetObject != null)
            {
                objectTransformGizmo.Gizmo.SetEnabled(true);
                objectTransformGizmo.SetTargetObject(targetObject);
            }
            else
            {
                objectTransformGizmo.Gizmo.SetEnabled(false);
            }
        }
    }
}
