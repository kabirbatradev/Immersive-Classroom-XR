using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class CamRotate : MonoBehaviour
{

    public Transform currentTarget;
    public float rotationSpeed = 300f;
    public float zoomSpeed = 10f;
    public float minZoomDistance = 1f;
    public float maxZoomDistance = 100f;
    private float distanceFromTarget;
    private Vector3 currentRotation;



    public static CamRotate Instance;
    private void Awake() {

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

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distanceFromTarget -= scrollInput * zoomSpeed;
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, minZoomDistance, maxZoomDistance);

        if (Input.GetMouseButton(1) && currentTarget != null)
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime * -1;
            float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentTarget.Rotate(Vector3.up, horizontalRotation, Space.World);
            currentTarget.Rotate(Vector3.right, verticalRotation, Space.Self);
        }

        Vector3 direction = new Vector3(0, 0, -distanceFromTarget);
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);

        if (currentTarget != null) {
            transform.position = currentTarget.position + rotation * direction;

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
