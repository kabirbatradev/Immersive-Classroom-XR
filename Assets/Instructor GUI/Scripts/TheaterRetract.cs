using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TheaterRetract : MonoBehaviour
{
    public float ceilingPercent = 0.0f;
    public float wallPercent = 0.0f;
    public GameObject[] ceilings;
    public GameObject[] walls;
    public Vector3[] originalCeilingPositions;
    public Vector3[] originalWallPositions;
    public Vector3[] originalCeilingScales;
    public void Start()
    {
        originalCeilingPositions = new Vector3[ceilings.Length];
        originalWallPositions = new Vector3[walls.Length];
        originalCeilingScales = new Vector3[ceilings.Length];
        for (int i = 0; i < ceilings.Length; i++)
        {
            originalCeilingPositions[i] = ceilings[i].transform.localPosition;
            originalCeilingScales[i] = ceilings[i].transform.localScale;
        }
        for (int i = 0; i < walls.Length; i++)
        {
            originalWallPositions[i] = walls[i].transform.localPosition;
        }
    }

    public void Update()
    {
        if (ceilingPercent >= 1.0f)
        {
            ceilingPercent = 1.0f;
        }
        if (wallPercent >= 1.0f)
        {
            wallPercent = 1.0f;
        }
        if (StreamTheaterModeData.Instance == null) {
            // hasnt joined room yet; return so no error every frame
            return;
        }
        ceilingPercent = StreamTheaterModeData.Instance.ceilingRemovedPercentage;
        wallPercent = StreamTheaterModeData.Instance.wallLoweredPercentage;
        retract(ceilingPercent, wallPercent);
    }

    public void retract(float ceilingPercent, float wallPercent)
    {
        foreach (GameObject ceiling in ceilings)
        {
            ceiling.transform.localPosition = new Vector3(100 * ceilingPercent,
            50 + wallPercent * -100.0f,
            ceiling.transform.localPosition.z);
            ceiling.transform.localScale = new Vector3(1.0f - ceilingPercent, 1.0f - ceilingPercent, 1.0f - ceilingPercent);
        }
        foreach (GameObject wall in walls)
        {
            wall.transform.localPosition = new Vector3(wall.transform.localPosition.x, wallPercent * -100.0f,
                wall.transform.localPosition.z);

            LineRenderer lineRenderer = wall.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = wall.AddComponent<LineRenderer>();
            }

            Vector3 topLeft = new Vector3(wall.transform.localPosition.x - (wall.transform.localScale.x / 2),
                wall.transform.localPosition.y + (wall.transform.localScale.y / 2), wall.transform.localPosition.z);
            Vector3 topRight = new Vector3(wall.transform.localPosition.x + (wall.transform.localScale.x / 2),
                wall.transform.localPosition.y + (wall.transform.localScale.y / 2), wall.transform.localPosition.z);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, topLeft);
            lineRenderer.SetPosition(1, topRight);

            // Set the line width
            lineRenderer.startWidth = 5f; // Adjust the thickness as needed
            lineRenderer.endWidth = 5f;

            // Set the line color to brown
            lineRenderer.startColor = new Color(0.6f, 0.3f, 0.0f);
            lineRenderer.endColor = new Color(0.6f, 0.3f, 0.0f);

            // Optionally set the material for better appearance
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    public void reset()
    {
        for (int i = 0; i < ceilings.Length; i++)
        {
            ceilings[i].transform.localPosition = originalCeilingPositions[i];
            ceilings[i].transform.localScale = originalCeilingScales[i];
        }
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].transform.localPosition = originalWallPositions[i];
        }
    }
}
