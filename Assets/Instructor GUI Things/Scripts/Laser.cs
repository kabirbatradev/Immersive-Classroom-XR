using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float laserRange = 100f;
    public float lineSize = 0.02f;
    private bool isShooting = false;
    public Camera mainCamera;
    public Material laserMaterial;
    public GameObject laserIndicator;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = laserMaterial;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
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
            laserIndicator.SetActive(false);
            lineRenderer.enabled = false;
        }

        if (isShooting)
        {
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        RaycastHit hit;
        lineRenderer.SetPosition(0, mainCamera.transform.position);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, laserRange) && hit.transform.gameObject != laserIndicator)
        {
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.enabled = true;
            laserIndicator.SetActive(true);
            laserIndicator.transform.position = hit.point;
        }
        else
        {
            lineRenderer.enabled = false;
            laserIndicator.SetActive(false);
        }
    }
}
