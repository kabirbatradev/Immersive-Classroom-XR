using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;

public class TheaterMode : MonoBehaviour
{
    private List<GameObject> allWalls = new List<GameObject>();
    private List<GameObject> ceilingRects = new List<GameObject>();
    private Vector3 originalPosition;
    private Vector3 finalPosition;

    private float timeLeft;
    private int index = 1;

    public Material[] skyboxMats;

    [SerializeField]
    private Transform controllerAnchor;

    [SerializeField]
    private float lineMaxLength = 50f;

    private RaycastHit hit;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update() {
        buildRoom();

        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {  
            Debug.Log("A WAS PRESSED, RETRACTING WALLS");
            StartCoroutine(StartCeilingMovement());
        }
        
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        { 
            ChangeSkybox(index);
        }
    }

    void buildRoom() 
    {
        Vector3 anchorPosition = controllerAnchor.position;
        Quaternion anchorRotation = controllerAnchor.rotation;

        if (Physics.Raycast(new Ray(anchorPosition, anchorRotation * Vector3.forward), out hit, lineMaxLength))
        {
            GameObject objectHit = hit.transform.gameObject;
            OVRSemanticClassification classification = objectHit?.GetComponentInParent<OVRSemanticClassification>();

            if (classification != null && classification.Contains(OVRSceneManager.Classification.Ceiling)) 
            {
                MeshRenderer rend = objectHit.GetComponent<MeshRenderer>();
                for (int i = 0; i < 4; i++) 
                {
                    GameObject clone = Instantiate(objectHit, objectHit.transform.position, objectHit.transform.rotation);
                    ceilingRects.Add(clone);
                }
                rend.enabled = false;
            }   
            if (classification != null && classification.Contains(OVRSceneManager.Classification.WallFace)) 
            {
                MeshRenderer rend = objectHit.GetComponent<MeshRenderer>();
                originalPosition = objectHit.transform.position;
                finalPosition = new Vector3(originalPosition.x, originalPosition.y - 0.5f, originalPosition.z);
                GameObject clone = Instantiate(objectHit, new Vector3(originalPosition.x, originalPosition.y, originalPosition.z), objectHit.transform.rotation);
                clone.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                allWalls.Add(clone);
                rend.enabled = false;
            } 
        }
    }

    void ChangeSkybox(int index) 
    {
        RenderSettings.skybox = skyboxMats[index];
        if (index == skyboxMats.Length)
        {
            index = 0;
        }
    }

    IEnumerator StartWallMovement() 
    {
        timeLeft = 4f;
        while (timeLeft >= 0.0f) {
            foreach (GameObject thisWall in allWalls) 
            {
                if (thisWall != null) 
                {
                    thisWall.transform.position += Vector3.down * 0.5f * Time.deltaTime;
                }
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator StartCeilingMovement() 
    {
        timeLeft = 9f;
        while (timeLeft >= 0.0f) 
        {
            for (int i = 0; i < ceilingRects.Count; i++) 
            {
                GameObject thisRect = ceilingRects[i];
                Transform child = thisRect.transform;
                if (i == 0) 
                {
                    child.Translate(Vector3.up * 0.5f * Time.deltaTime);
                }
                else if (i == 1)
                {
                    child.Translate(Vector3.down * 0.5f * Time.deltaTime);
                } 
                else if (i == 2)
                {
                    child.Translate(Vector3.right * 0.5f * Time.deltaTime);
                }
                else if (i == 3) {
                    child.Translate(Vector3.left * 0.5f * Time.deltaTime);
                }
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        foreach (GameObject thisRect in ceilingRects) 
        {
            Destroy(thisRect);
        }
        yield return StartCoroutine(StartWallMovement());
    }
}
