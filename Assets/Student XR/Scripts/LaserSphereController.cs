using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSphereController : MonoBehaviour
{

    private LineRenderer lineRenderer;
    private MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponentInParent<LineRenderer>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // place the sphere at the end of the line 

        Vector3 endPos = lineRenderer.GetPosition(1);
        gameObject.transform.position = endPos;

        // only show the sphere if lineRenderer.enabled
        meshRenderer.enabled = lineRenderer.enabled;

    }
}
