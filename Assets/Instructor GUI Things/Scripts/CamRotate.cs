using UnityEngine;
using System.Collections.Generic;

public class CamRotate : MonoBehaviour
{
    public List<Transform> targetObjects;
    private Transform currentTarget;
    public float rotationSpeed = 300f;
    public float zoomSpeed = 10f;
    public float minZoomDistance = 1f;
    public float maxZoomDistance = 100f;
    private float distanceFromTarget;
    private Vector3 currentRotation;

    void Start()
    {
        if (targetObjects.Count > 0)
        {
            currentTarget = targetObjects[0];
        }
        distanceFromTarget = Vector3.Distance(transform.position, currentTarget.position);
        currentRotation = transform.eulerAngles;
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distanceFromTarget -= scrollInput * zoomSpeed;
        distanceFromTarget = Mathf.Clamp(distanceFromTarget, minZoomDistance, maxZoomDistance);

        if (Input.GetMouseButton(1))
        {
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentRotation.x -= verticalInput;
            currentRotation.y += horizontalInput;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && targetObjects.Count >= 1)
        {
            currentTarget = targetObjects[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && targetObjects.Count >= 2)
        {
            currentTarget = targetObjects[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && targetObjects.Count >= 3)
        {
            currentTarget = targetObjects[2];
        }

        Vector3 direction = new Vector3(0, 0, -distanceFromTarget);
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        transform.position = currentTarget.position + rotation * direction;

        transform.LookAt(currentTarget);
    }
}
