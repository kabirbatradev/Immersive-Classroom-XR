using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    

    private const float wallDropPercentage = 0.8f;
    private const float wallMoveDuration = 4f;

    
    private List<GameObject> wallClones = new List<GameObject>();
    private List<GameObject> ceilingClones = new List<GameObject>();

    private List<Vector3> wallTargetPositions = new();


    // private List<GameObject> originalWallScenePlanes = new List<GameObject>();
    // private GameObject originalCeilingScenePlane;
    private List<GameObject> originalWalls = new List<GameObject>();
    private GameObject originalCeiling;

    private float wallHeight;



    // should be called automatically when the ScenePlane Objects are instantiated (by script AddToTheaterManager)
    public void AddScenePlane(GameObject scenePlaneObject) {

        OVRSemanticClassification classification = scenePlaneObject.GetComponent<OVRSemanticClassification>();

        if (classification == null) {
            // thats weird
            Debug.Log("scene plane object doesnt have a classification yet?");
        }   
        // if ceiling or walls
        else if (classification.Contains(OVRSceneManager.Classification.Ceiling) || classification.Contains(OVRSceneManager.Classification.WallFace)) {
            // get child object 
            // contains the mesh renderer of the passthrough
            GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
            
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
            // renderer.enabled = false;

            // does Instantiate keep all the properties? need to test --> no, i do need to pass in position and rotation
            GameObject clone = Instantiate(passthroughMeshObject, passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation); 

            if (classification.Contains(OVRSceneManager.Classification.Ceiling)) {
                // create 4 ceiling clones instead so that we can transition them outward and open the ceiling
                for (int i = 0; i < 4; i++) {
                    ceilingClones.Add(clone); 
                }

                // originalCeilingScenePlane = scenePlaneObject; // save the original object
                originalCeiling = passthroughMeshObject;
                
            }
            else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {
                wallClones.Add(clone);

                // originalWallScenePlanes.Add(scenePlaneObject); // save the original objects
                originalWalls.Add(passthroughMeshObject);

                // also update the height of the walls
                wallHeight = renderer.bounds.size.y;
            }
            renderer.enabled = false; // do not renderer the original passthrough meshes so we just see the clones and their movement
            // when we reset the theater, then we can destroy the clones and enable the renderer on these passthroughMeshObjects

            

        }
        // else if (classification.Contains(OVRSceneManager.Classification.Floor)) {
        //     GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
        //     MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
        //     renderer.enabled = false;
        //     // get rid of the renderer for now
        // }
    }



    void Start()
    {
        OVRManager.eyeFovPremultipliedAlphaModeEnabled = false;
        // required to see passthrough correctly
    }

    // // Update is called once per frame
    void Update() {
        
        // foreach (GameObject wall in wallClones) {
        //     if (wall != null) {
                // wall.transform.position += Vector3.down * 0.5f * Time.deltaTime;
                // wall.transform.Translate(speed * Time.deltaTime * Vector3.down);

                // Move towards is based on speed, not time
                // wall.transform.position = Vector3.MoveTowards(wall.transform.position, targetPosition, speed * Time.deltaTime);
                // or can use SmoothDamp

                // we should use lerp because it is based on getting the object there on time
                // wall.transform.position = Vector3.MoveTowards(wall.transform.position, targetPosition, speed * Time.deltaTime);

        //     }
        // }




        // A button pressed, debug logs
        if (OVRInput.GetDown(OVRInput.RawButton.A)) {  
            Debug.Log("A button was pressed");

            Debug.Log("number of ceilings: " + ceilingClones.Count);
            foreach (var clone in ceilingClones) {
                Debug.Log(clone);
            }
            Debug.Log("number of walls: " + wallClones.Count);
            foreach (var clone in wallClones) {
                Debug.Log(clone);
            }
            InitializeTheaterMode(); 
            TriggerTheaterMode();
        }


        if (OVRInput.GetDown(OVRInput.RawButton.B)) {  
            foreach (var wall in wallClones) {
                // wall.transform.Translate(Vector3.up * 1.0f)
                wall.transform.Translate(Vector3.up * Random.Range(0.0f, 2.0f));
            }
        }


    }


    private void InitializeTheaterMode() {
        float wallDistanceTravel = wallHeight * wallDropPercentage;
        wallTargetPositions.Clear();

        foreach (GameObject wall in originalWalls) {
            Vector3 targetPosition = wall.transform.position + Vector3.down * wallDistanceTravel;
            wallTargetPositions.Add(targetPosition);
        }
    }

    private void TriggerTheaterMode() {
        StartCoroutine(LowerWalls());
    }

    IEnumerator LowerWalls() {

        // start position of walls are stored in originalWalls list
        float timeElapsed = 0;
        float moveDuration = wallMoveDuration;

        while (timeElapsed < moveDuration) {
            // foreach (GameObject wall in wallClones) {
            for (int i = 0; i < wallClones.Count; i++) {
                GameObject wall = wallClones[i];

                Vector3 startPosition = originalWalls[i].transform.position;
                Vector3 targetPosition = wallTargetPositions[i];

                wall.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / moveDuration);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }



        // float timeLeft = 4f;
        // float speed = 0.5f;

        // while (timeLeft >= 0.0f) {
        //     foreach (GameObject wall in wallClones) {
        //         if (wall != null) {
        //             // wall.transform.position += Vector3.down * 0.5f * Time.deltaTime;
        //             wall.transform.Translate(speed * Time.deltaTime * Vector3.down);
        //         }
        //     }
        //     timeLeft -= Time.deltaTime;
        //     yield return null;
        // }
    }
}
