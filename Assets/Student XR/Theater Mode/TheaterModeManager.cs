using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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


    

    // private const float wallDropPercentage = 0.8f;
    // private const float wallMoveDuration = 4f;
    // private const float ceilingMoveDuration = 4f;

    // variables should be accessed from StreamTheaterModeData instance
    // private float currentWallLoweredPercentage = 0.5f;
    // private float currentCeilingRemovedPercentage = 0.5f;
    // private bool ceilingClonesActive = true;

    
    private List<GameObject> wallClones = new List<GameObject>();
    private List<GameObject> ceilingClones = new List<GameObject>();
    private GameObject floorClone = null;


    // no longer doing target positions; instead, it is computed every frame wrt og walls (because the original walls can drift with the anchors)
    // private List<Vector3> wallTargetPositions = new();
    // private List<Vector3> ceilingTargetPositions = new();


    // private List<GameObject> originalWallScenePlanes = new List<GameObject>();
    // private GameObject originalCeilingScenePlane;
    private List<GameObject> originalWalls = new List<GameObject>();
    private GameObject originalCeiling;
    private GameObject originalFloor;
    

    private float wallHeight;
    private float ceilingWidth;


    [SerializeField] 
    private GameObject PassthroughWallMesh;
    [SerializeField] 
    private GameObject PassthroughCeilingMesh;


    [SerializeField]
    private GameObject OVRSceneManagerObj;


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




        // update values for these from cloud
        float currentWallLoweredPercentage = 0.0f;
        float currentCeilingRemovedPercentage = 0.0f;
        bool ceilingClonesActive = true;
        // skybox current skybox:
        Material currentSkyboxMaterial = null;

        if (StreamTheaterModeData.Instance == null) {
            // do nothing, just use default values
        }
        else {
            currentWallLoweredPercentage = StreamTheaterModeData.Instance.wallLoweredPercentage;
            currentCeilingRemovedPercentage = StreamTheaterModeData.Instance.ceilingRemovedPercentage;
            ceilingClonesActive = StreamTheaterModeData.Instance.ceilingVisible;
            currentSkyboxMaterial = StreamTheaterModeData.GetSkyboxMaterial();
        }


        // set skybox material
        if (currentSkyboxMaterial != null) {
            RenderSettings.skybox = currentSkyboxMaterial;
        }

        
        // this means we dont own any clones, so they must be cloud objects controlled by another admin
        if (wallClones.Count == 0 && ceilingClones.Count == 0 && floorClone == null) {

            // so no need to update wall positions etc
            // but WE DO need to update the ceiling visibility
            GameObject[] ceilingMeshes = GameObject.FindGameObjectsWithTag("CeilingMesh"); // get ceiling objects (ceiling tag)
            foreach (GameObject ceiling in ceilingMeshes) {
                // ceiling clones should not be visible if ceilingClonesActive = false
                // ceiling.SetActive(ceilingClonesActive);
                ceiling.GetComponent<MeshRenderer>().enabled = ceilingClonesActive;
            }



            // the clones must have been disabled or destroyed (perhaps there is another admin)
            // therefore, no need to update positions of clones that don't exist
            return;
        }






        // set the position of each wall and ceiling clone with respect the current position of the original walls and ceiling
        // use currentWallLoweredPercentage to get how far the walls should be displaced
        // and currentCeilingRemovedPercentage for ceiling
        

        // update wall clone positions
        for (int i = 0; i < wallClones.Count; i++) {
            GameObject wall = wallClones[i];
            GameObject originalWall = originalWalls[i];

            Vector3 startPosition = originalWall.transform.position;

            Vector3 targetPosition = originalWall.transform.position + Vector3.down * wallHeight;

            // update the position of the clone
            // also update the rotation of the clone (for the chance that we changed the anchor alignment)
            wall.transform.SetPositionAndRotation(
                Vector3.Lerp(startPosition, targetPosition, currentWallLoweredPercentage), 
                originalWall.transform.rotation
            );

            Vector3 topLeft = wall.transform.localPosition + Quaternion.Euler(wall.transform.localEulerAngles) * new Vector3(-wall.transform.localScale.x / 2, wall.transform.localScale.y / 2, 0);
            Vector3 topRight = wall.transform.localPosition + Quaternion.Euler(wall.transform.localEulerAngles) * new Vector3(wall.transform.localScale.x / 2, wall.transform.localScale.y / 2, 0);
            
        }
        
        // update ceiling clone positions
        // Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right}; 
        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right}; 
        for (int i = 0; i < ceilingClones.Count; i++) {
            GameObject ceiling = ceilingClones[i];
            // GameObject originalCeiling

            // if ceiling clones should not be visible, then no need to set their position
            // ceiling.SetActive(ceilingClonesActive);
            ceiling.GetComponent<MeshRenderer>().enabled = ceilingClonesActive;

            if (!ceilingClonesActive) {
                continue;
            }





            // Vector3 targetPosition = originalCeiling.transform.position + directions[i] * ceilingWidth;
            // directions are often not in line with the actual ceiling since the room does not have to be axis aligned

            // if up down, set ceilingDistance to y scale
            // if left right, set ceilingDistance to x scale
            float ceilingDistance;
            if (i <= 1) {
                ceilingDistance = originalCeiling.transform.localScale.y;
            }
            else {
                ceilingDistance = originalCeiling.transform.localScale.x;
            }

            // mesh facing direction
            // Vector3 dir = originalCeiling.transform.TransformDirection(Vector3.forward);
            Vector3 moveDirection = originalCeiling.transform.TransformDirection(directions[i]);

            // target position should actually be approximately half way (because ceilings all begin overlapping)
            // Vector3 startPosition = originalCeiling.transform.position;
            Vector3 startPosition = originalCeiling.transform.position 
                + moveDirection * ceilingDistance * 0.48f; // less than 50% so even if its off by a bit, it still looks good

            // new direction code using transform direction
            // replace ceiling width with new ceilingDistance (width or length, uses scale instead of AA bounding box)
            Vector3 targetPosition = originalCeiling.transform.position 
                + moveDirection * ceilingDistance;
            

            // update the position of the clone
            // also update the rotation of the clone (for the chance that we changed the anchor alignment)
            ceiling.transform.SetPositionAndRotation(
                Vector3.Lerp(startPosition, targetPosition, currentCeilingRemovedPercentage), 
                originalCeiling.transform.rotation
            );
        }


        // update the floor clone position as well (make it match with the anchored floor)
        floorClone.transform.SetPositionAndRotation(
            originalFloor.transform.position, 
            originalFloor.transform.rotation
        );










        /*
        // A button pressed, debug logs
        if (OVRInput.GetDown(OVRInput.RawButton.A)) {  
            Debug.Log("A button was pressed");

            Debug.Log("number of ceilings: " + ceilingClones.Count);
            foreach (var clone in ceilingClones) {
                Debug.Log(clone);
            }
            Debug.Log("number of walls: " + wallClones.Count);
            foreach (var clone in wallClones) {
                Debug.Log(clone.transform.position);
            }

            // TriggerTheaterMode();
            // StartCoroutine(TestRemoveCeilingVariable());
        }
        */

        /*
        if (OVRInput.GetDown(OVRInput.RawButton.B)) {  
            foreach (var wall in wallClones) {
                // wall.transform.Translate(Vector3.up * 1.0f)
                wall.transform.Translate(Vector3.up * Random.Range(0.0f, 2.0f));
            }
        }
        */

        /*
        if (OVRInput.GetDown(OVRInput.RawButton.X)) {  
            // ResetTheaterMode();
        }
        */


    }

    // IEnumerator TestRemoveCeilingVariable() {

    //     // start position of walls are stored in originalWalls list
    //     float timeElapsed = 0;
    //     float moveDuration = 4.0f;

    //     while (timeElapsed < moveDuration) {
    //         currentCeilingRemovedPercentage = timeElapsed / moveDuration;

    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     StartCoroutine(TestLowerWallsVariable());
    // }

    // IEnumerator TestLowerWallsVariable() {

    //     // start position of walls are stored in originalWalls list
    //     float timeElapsed = 0;
    //     float moveDuration = 4.0f;

    //     while (timeElapsed < moveDuration) {
    //         currentWallLoweredPercentage = timeElapsed / moveDuration;

    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }
    // }


    


    // should be called automatically when the ScenePlane Objects are instantiated (by script AddToTheaterManager)
    public void AddScenePlane(GameObject scenePlaneObject) {

        OVRSemanticClassification classification = scenePlaneObject.GetComponent<OVRSemanticClassification>();
        if (classification == null) {
            // thats weird
            Debug.Log("scene plane object doesnt have a classification yet?");
        }

        GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 

        string type = null;
        if (classification.Contains(OVRSceneManager.Classification.Ceiling)) {
            type = "ceiling";
            originalCeiling = passthroughMeshObject;
        }
        else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {
            type = "wallFace";
            originalWalls.Add(passthroughMeshObject);
        }
        else if (classification.Contains(OVRSceneManager.Classification.Floor)) {
            type = "floor";
            originalFloor = passthroughMeshObject;
        }

        MakeCloneOfOriginalScenePlane(passthroughMeshObject, type);

        // // if ceiling or walls
        // else if (classification.Contains(OVRSceneManager.Classification.Ceiling)) {
        //     SampleController.Instance.Log("adding new Scene plane of classification: Ceiling");

        //     // get child object 
        //     // contains the mesh renderer of the passthrough
        //     GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
        //     MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();

        //     // create 4 ceiling clones instead so that we can transition them outward and open the ceiling
        //     for (int i = 0; i < 4; i++) {
        //         // new clone every time
                
        //         // GameObject clone = Instantiate(passthroughMeshObject, passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
        //         GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughCeilingMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
        //         // copy the scale to the clone too
        //         clone.transform.localScale = passthroughMeshObject.transform.localScale;
        //         ceilingClones.Add(clone); 
        //         // clone.GetComponent<MeshRenderer>().enabled = false;
        //     }

        //     // originalCeilingScenePlane = scenePlaneObject; // save the original object
        //     originalCeiling = passthroughMeshObject;

        //     // also update the width of the ceiling (set it to the larger value)
        //     ceilingWidth = Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y);
        //     Debug.Log("CEILING WIDTH: " + ceilingWidth);
        //     // the bounding box is axis aligned so, this may not work
        //     // TODO?

        //     renderer.enabled = false; // do not renderer the original passthrough meshes so we just see the clones and their movement
        // }
        // else if (classification.Contains(OVRSceneManager.Classification.WallFace)) {
        //     SampleController.Instance.Log("adding new Scene plane of classification: WallFace");

        //     GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
        //     MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();

        //     // GameObject clone = Instantiate(passthroughMeshObject, passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation); 
        //     GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughWallMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation); 
            
        //     // clone.transform.localScale += new Vector3(-0.1f,-0.1f,-0.1f);
        //     // clone.transform.localScale += new Vector3(0.001f,0.001f,0.001f);
        //     // clone.transform.localScale += new Vector3(0.01f,0.01f,0.01f);
        //     // clone.transform.localScale += new Vector3(0.003f,0.003f,0.003f);
        //     clone.transform.localScale += new Vector3(0.005f,0.005f,0.005f); // scale up the walls a tiny bit so that there is no tiny edge between walls with no passthrough
        //     // copy the scale to the clone too
        //     clone.transform.localScale = passthroughMeshObject.transform.localScale;
        //     wallClones.Add(clone);

        //     // originalWallScenePlanes.Add(scenePlaneObject); // save the original objects
        //     originalWalls.Add(passthroughMeshObject);

        //     // also update the height of the walls
        //     wallHeight = renderer.bounds.size.y;

        //     renderer.enabled = false;
        // }
        // else if (classification.Contains(OVRSceneManager.Classification.Floor)) {
        //     SampleController.Instance.Log("adding new Scene plane of classification: Floor");

        //     GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
        //     MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
        //     renderer.enabled = false; // do not render the original floor because we will clone it
            
        //     // we can use the wall mesh again for the floor
        //     GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughWallMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
        //     // copy the scale to the clone too
        //     clone.transform.localScale = passthroughMeshObject.transform.localScale;

        //     originalFloor = passthroughMeshObject;

        //     floorClone = clone;
        // }
        // else {
        //     // what classification is this?
        //     SampleController.Instance.Log("unexpected scene plane classification");

        //     GameObject passthroughMeshObject = scenePlaneObject.transform.GetChild(0).gameObject; 
        //     MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
        //     renderer.enabled = false;
        //     // disable its renderer so nothing unexpected happens
        // }
    }

    

    public void DestroyScenePlaneClones() {
        
        SampleController.Instance.Log("DestroyScenePlaneClones was called");

        // get all walls etc
        // for each, take photon ownership
        // then photon destroy

        // also clear the wallclones and ceiling clones arrays etc

        var walls = GameObject.FindGameObjectsWithTag("WallMesh");
        var ceilings = GameObject.FindGameObjectsWithTag("CeilingMesh");

        var totalCount = 0;

        foreach (var wall in walls) {
            // SampleController.Instance.Log($"Photon view: {wall.GetPhotonView().ToString()}");
            if (wall.GetPhotonView().ViewID == 0) continue;
            wall.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.Destroy(wall);
            totalCount++;
        }

        foreach (var ceil in ceilings) {
            // SampleController.Instance.Log($"Photon view: {ceil.GetPhotonView().ToString()}");
            if (ceil.GetPhotonView().ViewID == 0) continue;
            ceil.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.Destroy(ceil);
            totalCount++;
        }

        wallClones.Clear();
        ceilingClones.Clear();
        floorClone = null;

        SampleController.Instance.Log($"Destroy {totalCount} scene plane meshes");

    }


    public void CreateScenePlaneClones() {

        SampleController.Instance.Log("CreateScenePlaneClones was called");
        
        // first, call destroy on all walls etc
        DestroyScenePlaneClones();

        // if the scene manager is not enabled, then enable it, and AddScenePlane should be automatically called, so return
        if (!OVRSceneManagerObj.activeSelf) {
            SampleController.Instance.Log("setting OVRSceneManager to active to automatically create walls");
            OVRSceneManagerObj.SetActive(true);
            return;
        }

        // otherwise, go through all the original walls and ceilings and floors etc and make clones of them
        // if they dont exist, give an error message

        // originalCeiling
        // originalFloor
        // originalWalls
        int totalCount = originalWalls.Count + 2;

        SampleController.Instance.Log($"cloning {totalCount} scene plane meshes (+3 for duplicate ceilings)");

        // go through all the originals and clone them
        // call MakeCloneOfOriginalScenePlane
        MakeCloneOfOriginalScenePlane(originalCeiling, "ceiling");
        MakeCloneOfOriginalScenePlane(originalFloor, "floor");
        foreach (var wall in originalWalls) {
            MakeCloneOfOriginalScenePlane(wall, "wallFace");
        }
        
    }


    private void MakeCloneOfOriginalScenePlane(GameObject originalScenePlaneMesh, string type) {
        SampleController.Instance.Log($"cloning scene plane: {type}");

        if (type == null) {
            // thats weird
            Debug.Log("Error: scene plane object type is null");
        }   
        else if (type == "ceiling") {

            GameObject passthroughMeshObject = originalScenePlaneMesh;
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();

            // create 4 ceiling clones instead so that we can transition them outward and open the ceiling
            for (int i = 0; i < 4; i++) {
                // new clone every time
                
                // GameObject clone = Instantiate(passthroughMeshObject, passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
                GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughCeilingMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
                // copy the scale to the clone too
                clone.transform.localScale = passthroughMeshObject.transform.localScale;
                ceilingClones.Add(clone); 
            }

            // also update the width of the ceiling (set it to the larger value)
            ceilingWidth = Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.y);
            Debug.Log("CEILING WIDTH (bounding box x): " + renderer.bounds.size.x);
            Debug.Log("CEILING WIDTH (bounding box z): " + renderer.bounds.size.z);
            Debug.Log("CEILING WIDTH (scale x): " + passthroughMeshObject.transform.localScale.x);
            Debug.Log("CEILING WIDTH (scale y): " + passthroughMeshObject.transform.localScale.y);
            // the bounding box is axis aligned so, this may not work
            // TODO: figure out the direction the ceiling's edges are facing to translate them in the right direction in theater mode

            renderer.enabled = false; // do not renderer the original passthrough meshes so we just see the clones and their movement
        }
        else if (type == "wallFace") {

            GameObject passthroughMeshObject = originalScenePlaneMesh;
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();

            // GameObject clone = Instantiate(passthroughMeshObject, passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation); 
            GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughWallMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation); 
            
            // clone.transform.localScale += new Vector3(-0.1f,-0.1f,-0.1f);
            // clone.transform.localScale += new Vector3(0.001f,0.001f,0.001f);
            // clone.transform.localScale += new Vector3(0.01f,0.01f,0.01f);
            // clone.transform.localScale += new Vector3(0.003f,0.003f,0.003f);
            // copy the scale to the clone too
            clone.transform.localScale = passthroughMeshObject.transform.localScale;
            clone.transform.localScale += new Vector3(0.005f,0.005f,0.005f); // scale up the walls a tiny bit so that there is no tiny edge between walls with no passthrough
            // Show the wall thickness
            clone.GetComponent<PhotonView>().RPC("ActivateThickness", RpcTarget.AllBuffered, clone.transform.localScale.x);
            
            // GameObject thickness = clone.transform.GetChild(0).gameObject;
            // thickness.SetActive(true);
            // thickness.GetComponent<Renderer>().material.mainTextureScale = new Vector2(clone.transform.localScale.x, 1);
            
            wallClones.Add(clone);

            // also update the height of the walls
            wallHeight = renderer.bounds.size.y;

            renderer.enabled = false;
        }
        else if (type == "floor") {

            GameObject passthroughMeshObject = originalScenePlaneMesh;
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
            renderer.enabled = false; // do not render the original floor because we will clone it
            
            // we can use the wall mesh again for the floor
            GameObject clone = PhotonNetwork.Instantiate(nameof(PassthroughWallMesh), passthroughMeshObject.transform.position, passthroughMeshObject.transform.rotation);
            // copy the scale to the clone too
            clone.transform.localScale = passthroughMeshObject.transform.localScale;

            floorClone = clone;
        }
        else {
            // what classification is this?
            SampleController.Instance.Log("Error: scene plane object type is invalid");

            GameObject passthroughMeshObject = originalScenePlaneMesh; 
            MeshRenderer renderer = passthroughMeshObject.GetComponent<MeshRenderer>();
            renderer.enabled = false;
            // disable its renderer so nothing unexpected happens
        }


    }



    // old theater mode code below ---------


    /*

    private void ResetTheaterMode() {
        InitializeTheaterMode();
    }


    private void InitializeTheaterMode() {
        float wallDistanceTravel = wallHeight * wallDropPercentage;
        
        // set wallTargetPositions
        wallTargetPositions.Clear();
        // foreach (GameObject wall in originalWalls) {
        for (int i = 0; i < originalWalls.Count; i++) {
            GameObject originalWall = originalWalls[i];
            Vector3 targetPosition = originalWall.transform.position + Vector3.down * wallDistanceTravel;
            wallTargetPositions.Add(targetPosition);

            // reset the wall positions
            // essentially we are recloning the walls because we want to reset them and also if anchor alignment changed, then we need to reset even the rotation
            wallClones[i].transform.rotation = originalWall.transform.rotation;
            wallClones[i].transform.position = originalWall.transform.position;
        }

        // perhaps the start positions should be set so that there is no overlap so that the ceiling opens immediately when the button is pressed
        // float ceilingDistanceTravel = ceilingWidth * 0.5f; // it does go a little past but just for a frame or 2... i think it does depend on the alignment of the coordinate system
        float ceilingDistanceTravel = ceilingWidth * 1f; 
        // float ceilingDistanceTravel = ceilingWidth * 1.2f; // doesnt hurt to overshoot the ceiling distance a little (usually the ceiling direction is wrong, thats a future fix)
        // float ceilingDistanceTravel = 5f; 

        // set ceilingTargetPositions
        ceilingTargetPositions.Clear();
        Vector3[] directions = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
        for (int i = 0; i < ceilingClones.Count; i++) {
            Vector3 targetPosition = originalCeiling.transform.position + directions[i] * ceilingDistanceTravel;
            ceilingTargetPositions.Add(targetPosition);

            // Debug.Log("target ceiling position target: " + targetPosition);

            // reset the ceiling positions
            ceilingClones[i].transform.position = originalCeiling.transform.position;
            ceilingClones[i].transform.rotation = originalCeiling.transform.rotation;

            // make sure all ceiling clones are visible as they are made invisible after running "RemoveCeiling"
            ceilingClones[i].GetComponent<MeshRenderer>().enabled = true;
        }

    }

    private void TriggerTheaterMode() {
        Debug.Log("triggering theater mode");
        InitializeTheaterMode(); 
        StartCoroutine(RemoveCeiling());
    }

    IEnumerator RemoveCeiling() {

        Debug.Log("removing ceiling");
        Debug.Log("COUNT " + ceilingClones.Count); // is 4

        // start position of walls are stored in originalWalls list
        float timeElapsed = 0;
        float moveDuration = ceilingMoveDuration;

        while (timeElapsed < moveDuration) {
            for (int i = 0; i < ceilingClones.Count; i++) {
                GameObject ceiling = ceilingClones[i];

                Vector3 startPosition = originalCeiling.transform.position;
                Vector3 targetPosition = ceilingTargetPositions[i];

                ceiling.transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / moveDuration);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < ceilingClones.Count; i++) {
            ceilingClones[i].GetComponent<MeshRenderer>().enabled = false; // make the ceiling invisible
        }
        yield return StartCoroutine(LowerWalls());
        
    }

    IEnumerator LowerWalls() {

        Debug.Log("lowering walls");

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

    */
    

}
