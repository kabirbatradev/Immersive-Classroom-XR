using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheaterModeManager : MonoBehaviour
{

    // make this a singleton class so that we can access this script/object globally (when walls objects are instantiated, they can add themselves to the list automatically)
    public static TheaterModeManager Instance;
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    // end singleton setup


    
    private List<GameObject> wallClones = new List<GameObject>();
    private List<GameObject> ceilingClone = new List<GameObject>();


    private List<GameObject> originalWallScenePlanes = new List<GameObject>();
    private GameObject originalCeilingScenePlane;



    // should be called automatically when the ScenePlane Objects are instantiated (by script AddToTheaterManager)
    public void AddScenePlane(GameObject scenePlaneObject) {

        OVRSemanticClassification classification = scenePlaneObject.GetComponent<OVRSemanticClassification>();

        if (classification == null) {
            // thats weird
            Debug.Log("scene plane object doesnt have a classification yet?");
        }   
        // if ceiling or walls
        else if (classification.Contains(OVRSceneManager.Classification.Ceiling) || classification.Contains(OVRSceneManager.Classification.WallFace)) {
            GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; // contains the mesh renderer of the pass
            
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
            // renderer.enabled = false;

            // GameObject clone = Instantiate(passthroughMeshObject); // does Instantiate keep all the properties? need to test

            if (classification.Contains(OVRSceneManager.Classification.Ceiling)) {
                GameObject clone = Instantiate(passthroughMeshObject); // does Instantiate keep all the properties? need to test
                ceilingClone.Add(clone); // create 4 ceiling clones instead

                originalCeilingScenePlane = scenePlaneObject; // save the original object
            }
            else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {
                GameObject clone = Instantiate(passthroughMeshObject); // does Instantiate keep all the properties? need to test
                wallClones.Add(clone);

                originalWallScenePlanes.Add(scenePlaneObject); // save the original objects
            }
            renderer.enabled = false; // do not renderer the original passthrough meshes so we just see the clones and their movement
            // when we reset the theater, then we can destroy the clones and enable the renderer on these passthroughMeshObjects

            

        }
        // else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {

        //     GameObject sceneMeshObject = scenePlaneObject.transform.GetChild(0).gameObject;
        //     MeshRenderer renderer = sceneMeshObject.GetComponent<MeshRenderer>();
        //     wallClones.Add(sceneMeshObject);

        // }
    }



    void Start()
    {
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
    }

    // // Update is called once per frame
    void Update() {

        // A button pressed, debug logs
        if (OVRInput.GetDown(OVRInput.RawButton.A)) {  
            Debug.Log("A button was pressed");

            Debug.Log("number of ceilings: " + ceilingClone.Count);
            foreach (var clone in ceilingClone) {
                Debug.Log(clone);
            }
            Debug.Log("number of walls: " + wallClones.Count);
            foreach (var clone in wallClones) {
                Debug.Log(clone);
            }
        }


        if (OVRInput.GetDown(OVRInput.RawButton.B)) {  
            foreach (var wall in wallClones) {
                // wall.transform.Translate(Vector3.up * 1.0f)
                wall.transform.Translate(Vector3.up * Random.Range(0.0f, 2.0f));
            }
        }


    }
}
