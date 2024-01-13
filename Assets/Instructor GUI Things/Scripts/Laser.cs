using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float laserRange = 100f;
    public float lineSize = 0.02f;
    private bool isShooting = false;
    public Camera mainCamera;
    public GameObject laserStartPoint;
    public Material laserMaterial;
    private static bool laserOn = true;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = laserMaterial;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
            // set to layer "mainObject"
            lineRenderer.gameObject.layer = 7;
        }

        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            lineRenderer.enabled = false;
        }

        if (isShooting && laserOn)
        {
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        RaycastHit hit;
        // Set the start position to be the position of the laserStartPoint
        lineRenderer.SetPosition(0, laserStartPoint.transform.position);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, laserRange))
        {
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public static void ToggleLaser(bool status)
    {
        {
            laserOn = status;
        }
    }
}
