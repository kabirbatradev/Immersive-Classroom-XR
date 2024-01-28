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



    // should be called automatically when the ScenePlane Objects are instantiated (by script AddToTheaterManager)
    public void AddScenePlane(GameObject scenePlaneObject) {

        OVRSemanticClassification classification = scenePlaneObject.GetComponent<OVRSemanticClassification>();

        if (classification == null) {
            // thats weird
            Debug.Log("scene plane object doesnt have a classification yet?");
        }
        else if (classification.Contains(OVRSceneManager.Classification.Ceiling)) {
            GameObject sceneMeshObject = scenePlaneObject.transform.GetChild(0).gameObject;
            
            MeshRenderer renderer = sceneMeshObject.GetComponent<MeshRenderer>();
            renderer.enabled = false; // do not render the passthrough mesh

            ceilingClone.Add(sceneMeshObject);

                // originalPosition = objectHit.transform.position;
                // finalPosition = new Vector3(originalPosition.x, originalPosition.y - 0.5f, originalPosition.z);
                // GameObject clone = Instantiate(objectHit, new Vector3(originalPosition.x, originalPosition.y, originalPosition.z), objectHit.transform.rotation);
                // clone.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                // allWalls.Add(clone);
                // rend.enabled = false;

        }
        else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {

            GameObject sceneMeshObject = scenePlaneObject.transform.GetChild(0).gameObject;
            MeshRenderer renderer = sceneMeshObject.GetComponent<MeshRenderer>();
            wallClones.Add(sceneMeshObject);

        }
    }



    void Start()
    {
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
    }

    // // Update is called once per frame
    void Update() {
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
    }
}
