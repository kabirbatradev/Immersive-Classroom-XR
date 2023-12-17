using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class CamRotate : MonoBehaviour
{

    public Transform currentTarget;
    public float rotationSpeed = 300f;
    public float zoomSpeed = 0.5f;
    public float minZoomDistance = 1f;
    public float maxZoomDistance = 5f;
    private float distanceFromTarget;
    private Vector3 currentRotation;

    public static CamRotate Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    void Start()
    {
        currentRotation = transform.eulerAngles;
        currentTarget = null;
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);
    }

    void Update()
    {
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        if (mainObjectContainer != null && RoomHasCustomProperty("mainObjectCurrentModelName"))
        {
            string currentActiveObjectName = (string)GetRoomCustomProperty("mainObjectCurrentModelName");
            Transform foundTarget = FindTargetByName(currentActiveObjectName);
            if (foundTarget != null)
            {
                currentTarget = foundTarget;
            }
        }
        else
        {
            return;
        }

        if (currentTarget != null && Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            currentRotation.y += mouseX * rotationSpeed * Time.deltaTime;
            currentRotation.x += mouseY * rotationSpeed * Time.deltaTime;
            currentRotation.x = Mathf.Clamp(currentRotation.x, -90f, 90f); // Limit vertical rotation
            Vector3 nextPosition = currentTarget.position - Quaternion.Euler(currentRotation) * Vector3.forward * distanceFromTarget;
            transform.position = nextPosition;
            transform.LookAt(currentTarget);
        }
    }

    private Transform FindTargetByName(string targetName)
    {
        // find it in the main object container
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        if (mainObjectContainer != null)
        {
            foreach (Transform child in mainObjectContainer.transform)
            {
                if (child.name == targetName)
                {
                    return child;
                }
            }
        }
        return null;
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
