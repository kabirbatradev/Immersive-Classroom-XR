
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
using Photon.Pun;
// using PlayerProperties = Photon.Pun.PhotonNetwork.CustomProperties;
// using PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;
// using LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;

public class SharedAnchorControlPanelAdditionalFunctions : MonoBehaviour
{

    public const int defaultGroupNumber = 1;
    // const implies static too


    [SerializeField]
    private bool isInstructorGUIToggle;





    [SerializeField]
    private GameObject spherePrefab;


    [SerializeField]
    private GameObject jengaPrefab;

    [SerializeField]
    private GameObject alignedTablePrefab;


    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject mainObjectContainerPrefab;



    // [SerializeField]
    // private GameObject[] adminButtons;

    // [SerializeField]
    // private GameObject[] studentButtons;


    private bool alignTableMode = false;
    // private int countAButton = 0;
    private List<Vector3> tablePoints = new();
    [SerializeField]
    private Transform rightControllerTransform;

    // [SerializeField] private OVRSpatialAnchor anchorPrefab;




    private GameObject mostRecentSphere;





    // private LineRenderer lineRenderer;
    // private float lineSize = 0.02f;
    // public Material laserMaterial;

    private List<GameObject> laserGameObjects;

    [SerializeField] 
    private GameObject laserGameObjectPrefab;


    [SerializeField]
    private GameObject groupNumberTextObject;


    private int nextDeskRowNumber = 1;

    [SerializeField]
    private GameObject seatMarkerPrefab;


    // [SerializeField]
    // private GameObject OVRSceneManagerObj;

    private enum DeviceModes {Admin, Student, Camera};
    private DeviceModes deviceCurrentMode = DeviceModes.Student;

    


    public void Start() {
        laserGameObjects = new List<GameObject>();
        // initialize laser renderer

        // if (lineRenderer == null) {
        //     lineRenderer = gameObject.AddComponent<LineRenderer>();
        //     lineRenderer.material = laserMaterial;
        //     lineRenderer.startWidth = lineSize;
        //     lineRenderer.endWidth = lineSize;


        //     lineRenderer.startColor = Color.red;
        //     lineRenderer.endColor = Color.red;
        // }

        // lineRenderer.enabled = false;


        // if (isInstructorGUIToggle) {
        //     defaultGroupNumber = 0;
        // }
    } 


    public void Update() {


        if (PhotonPun.PhotonNetwork.CurrentRoom == null) {
            return;
        }

        // /*
        if (alignTableMode) {

            // bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
            bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.A);

            
            if (buttonPressed) {
                SampleController.Instance.Log("The A button was pressed!");

                // var controllerType = OVRInput.Controller.RTouch; // the right controller
                // Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controllerType);
                // Quaternion controllerRotation = OVRInput.GetLocalControllerRotation(controllerType);
                Vector3 controllerPosition = rightControllerTransform.position;
                Quaternion controllerRotation = rightControllerTransform.rotation;


                string x = controllerPosition.x.ToString("0.00");
                string y = controllerPosition.y.ToString("0.00");
                string z = controllerPosition.z.ToString("0.00");
                SampleController.Instance.Log(x + " " + y + " " + z);

                tablePoints.Add(controllerPosition);

                // create a spatial anchor at the point so we can lock the table to these anchors
                // Instantiate(anchorPrefab, controllerPosition, controllerRotation).GetComponent<SharedAnchor>();
                // nvm im going to create the anchor on a script on the table itself

                // countAButton++;
                if (tablePoints.Count == 3) {
                    // countAButton = 0;
                    alignTableMode = false;
                    SampleController.Instance.Log("Creating Table...");
                    InstantiateAlignedTable(tablePoints[0], tablePoints[1], tablePoints[2]);
                    tablePoints.Clear();
                }
            }
        }
        // */


        /*
        // if the B button is pressed, then enable or disable the most recent spawned sphere
        bool BButton = OVRInput.GetDown(OVRInput.RawButton.B);
        if (BButton) {
            
            // mostRecentSphere.SetActive(!mostRecentSphere.activeSelf);

            // instead, lets change the group number and see if the code below works 
            // (automatically set inactive if not same group number)
            ObjectData data = mostRecentSphere.GetComponent<ObjectData>();
            data.groupNumber = data.groupNumber == 0 ? 1 : 0;
            SampleController.Instance.Log("set group number to " + data.groupNumber);
        }
        */







        
        // Object Group Filtering:

        // every frame, scan through all objects that have a photon id (photon view)
        // and enable or disable them based on if their group number matches the current 
        // user's group number

        // also disable passthrough wall meshes for instructor 
        
        
        int currentUserGroupNumber = GetCurrentGroupNumber(); // gets group number from local player's custom propreties
        if (isInstructorGUIToggle) currentUserGroupNumber = 0; // this hardcode is also in the GetCurrentGroupNumber function
        // instructor group number = 0 allows for instructor to view every instance of objects

        // need to include inactive 
        // ObjectData[] allNetworkObjectDatas = (ObjectData[]) FindObjectsByType<ObjectData>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        PhotonPun.PhotonView[] allPhotonViews = (PhotonPun.PhotonView[]) FindObjectsByType<PhotonPun.PhotonView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (PhotonPun.PhotonView photonView in allPhotonViews) {
            GameObject obj = photonView.gameObject;
            if (obj == null) {
                // SampleController.Instance.Log("photonView was attached to null object");
                // Debug.Log("photonView was attached to null object");
                continue;
            }


            // if is instructor and this object is a passthrough mesh, then disable
            if (isInstructorGUIToggle && (obj.CompareTag("WallMesh") || obj.CompareTag("CeilingMesh"))) {
                obj.SetActive(false);
            }


            // use photonView viewID as key to room custom properties and get group number
            // if no group number, then skip (some objects dont have group numbers)
            if (!PhotonObjectHasGroupNumber(obj)) continue; // error happening here? object reference not set to instance of object 
            // seems to give error when we arent in a room yet
            int objectGroupNumber = GetPhotonObjectGroupNumber(obj); 

            // if group 0 or the group numbers match, then set the object to active
            if (currentUserGroupNumber == 0 || objectGroupNumber == currentUserGroupNumber) {
                obj.SetActive(true);
            }
            // if they do not match, disable the object
            else {
                obj.SetActive(false);
            }
        }


        /*
        // if there is an active "MainObjectContainer" in the scene
        // then change check the room custom properties to see which model should be active, and deactive the rest

        // not recommended to use Find every frame (instead, we should cache the object)
        // or use FindWithTag(tag) --> returns 1 active game object or null if dne
        // tag is "MainObjectContainer"
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        GameObject currentActiveGameObject = null;
        if (mainObjectContainer != null) {
            if (RoomHasCustomProperty("mainObjectCurrentModelName")) {
                string currentActiveObjectName = (string)GetRoomCustomProperty("mainObjectCurrentModelName");
                
                // for every potential model (child of container), disable unless name = currentActiveObject
                foreach (Transform child in mainObjectContainer.transform) {
                    GameObject potentialModel = child.gameObject;
                    // if (potentialModel.name == currentActiveObject)
                    potentialModel.SetActive(potentialModel.name == currentActiveObjectName);
                    // Debug.Log(potentialModel.name);
                    if (potentialModel.name == currentActiveObjectName) {
                        currentActiveGameObject = potentialModel;
                    }
                }
            }

        }
        */







        /*



        // simply get the current active game object so it can be rotated by the instructor
        GameObject currentActiveGameObject = null;
        
        GameObject mainObjectContainer = GameObject.FindWithTag("MainObjectContainer");
        // if the main object exists
        if (mainObjectContainer != null) {
            // iterate through all children
            foreach (Transform child in mainObjectContainer.transform) {
                // find the child that is active
                GameObject potentialModel = child.gameObject;
                if (potentialModel.activeSelf) {
                    currentActiveGameObject = potentialModel;
                    break;
                }
            }
        }


        // if main object exists, check if the professor wants to 
        // rotate it
        // scale it
        // draw a laser

        // ONLY DRAW THE LASER AND UPDATE ROTATION IF THIS IS A HEADSET (NOT INSTRUCTOR GUI)
        if (mainObjectContainer != null && !isInstructorGUIToggle) {
            bool isShootingExists = RoomHasCustomProperty("IsShooting");

            // if (isShootingExists != isShootingExisted) {
            //     isShootingExisted = isShootingExists;
            //     SampleController.Instance.Log("isShootingExists is now: " + isShootingExists);

            //     if (isShootingExists) {
            //         bool isShooting = (bool)GetRoomCustomProperty("IsShooting");
            //         SampleController.Instance.Log("isShooting is: " + isShooting);
            //     }
            // }

            
            
            bool objectRotationExists = RoomHasCustomProperty("ObjectRotation");
            if (objectRotationExists) {



                // get rotation from server side and rotate the main object
                // bool objectRotationExist = RoomHasCustomProperty("ObjectRotation");
                Quaternion objectRotation = (Quaternion)GetRoomCustomProperty("ObjectRotation");
                // SampleController.Instance.Log("rotation is: " + objectRotation);
                currentActiveGameObject.transform.rotation = objectRotation;




                // if rotation exists, then scaling probably also exists
                Vector3 objectScale = (Vector3)GetRoomCustomProperty("ObjectScale");
                currentActiveGameObject.transform.localScale = objectScale;



                // if the professor is shooting a laser, then we should see it
                if (isShootingExists) {
                    bool isShooting = (bool)GetRoomCustomProperty("IsShooting");
                    // SampleController.Instance.Log("isShooting is: " + isShooting);
                    if (isShooting) {
                        // get camera position and hit position
                        Vector3 cameraPosition = (Vector3)GetRoomCustomProperty("CameraPosition");
                        Vector3 hitPosition = (Vector3)GetRoomCustomProperty("HitPosition");

                        // lineRenderer is the line renderer
                        // use lineRenderer.SetPosition(index, vector3); for index 0 and 1
                        // enable or disable the line renderer

                        // draw the line with respect to the main object
                            // rotation and position
                        lineRenderer.enabled = true;
                        lineRenderer.SetPosition(0, cameraPosition + currentActiveGameObject.transform.position);
                        lineRenderer.SetPosition(1, hitPosition + currentActiveGameObject.transform.position);

                    }
                    else {
                        lineRenderer.enabled = false;
                    }
                }
            }

            // // test if line renderer can render with respect to the main object (works)
            // lineRenderer.enabled = true;
            // lineRenderer.SetPosition(0, new Vector3(0, 0, 0) + currentActiveGameObject.transform.position);
            // lineRenderer.SetPosition(1, new Vector3(1, 0, 0) + currentActiveGameObject.transform.position);

        }

        */


        // GameObject laserGameObject = Instantiate(laserGameObjectPrefab);
        // LineRenderer lineRenderer = laserGameObject.GetComponent<LineRenderer>();
        // // test if line renderer is working (works)
        // lineRenderer.enabled = true;
        // lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        // lineRenderer.SetPosition(1, new Vector3(1, 1, 1));





        // iterate through all main object instances, only show the correct subobject, 
        // draw the instructor laser pointer, and update rotation and scaling of the object

        // get all objects
        GameObject[] mainObjectContainers = GameObject.FindGameObjectsWithTag("MainObjectContainer");

        // get the name of the currently active model from the server
        string currentActiveObjectName = null;
        if (RoomHasCustomProperty("mainObjectCurrentModelName")) {
            currentActiveObjectName = (string)GetRoomCustomProperty("mainObjectCurrentModelName");
        }

        // iterate through all objects
        int mainObjectIndex = -1;
        foreach (GameObject mainObjectContainer in mainObjectContainers) {
            mainObjectIndex++; // to be used with the list of line renderers
            if (mainObjectContainer == null) continue;

            // save the active model in this variable
            GameObject currentActiveModel = null;

            // if there is a server variable determining which model should be active, then only enable that model
            if (currentActiveObjectName != null) {
                // for every potential model (child of container), disable unless name = currentActiveObject
                foreach (Transform child in mainObjectContainer.transform) {
                    GameObject potentialModel = child.gameObject;
                    potentialModel.SetActive(potentialModel.name == currentActiveObjectName);
                    if (potentialModel.name == currentActiveObjectName) {
                        currentActiveModel = potentialModel;
                    }
                }
            }
            // if there isnt a server variable, then iterate through and pick out the active model
            else {
                foreach (Transform child in mainObjectContainer.transform) {
                    GameObject potentialModel = child.gameObject;
                    if (potentialModel.activeSelf) {
                        currentActiveModel = potentialModel;
                        break;
                    }
                }
            }

            // given this active model, lets draw the laser, rotate the object, and scale the object
            // this should only be done on the device side (not the instructor side)
            if (!isInstructorGUIToggle) {
                bool objectRotationExists = RoomHasCustomProperty("ObjectRotation");
                if (objectRotationExists) {

                    // get rotation from server side and rotate the main object
                    Quaternion objectRotation = (Quaternion)GetRoomCustomProperty("ObjectRotation");
                    currentActiveModel.transform.rotation = objectRotation;

                    // if rotation exists, then scaling also exists (they are set at the same time)
                    Vector3 objectScale = (Vector3)GetRoomCustomProperty("ObjectScale");
                    currentActiveModel.transform.localScale = objectScale;

                    // create a new laser instance if it doesnt already exist
                    if (mainObjectIndex >= laserGameObjects.Count) {
                        laserGameObjects.Add(Instantiate(laserGameObjectPrefab));
                    }

                    // get the corresponding laser object to this main object at this main object index
                    GameObject laserInstance = laserGameObjects[mainObjectIndex];

                    // get the laser prefab's line renderer to be used to draw the laser if the professor is shooting a laser
                    LineRenderer lineRenderer = laserInstance.GetComponent<LineRenderer>();

                    // if the professor is shooting a laser, then we should see it
                    bool isShootingExists = RoomHasCustomProperty("IsShooting");
                    // Debug.Log("is shooting exists? " + isShootingExists);
                    if (isShootingExists) {
                        bool isShooting = (bool)GetRoomCustomProperty("IsShooting");
                        // Debug.Log("is shooting equals: " + isShooting);
                        if (isShooting) {
                            // get camera position and hit position
                            Vector3 cameraPosition = (Vector3)GetRoomCustomProperty("CameraPosition");
                            Vector3 hitPosition = (Vector3)GetRoomCustomProperty("HitPosition");
                            // Debug.Log("camera position " + cameraPosition);
                            // Debug.Log("hit position " + hitPosition);

                            // use lineRenderer.SetPosition(index, vector3); for index 0 and 1
                            // enable or disable the line renderer

                            // draw the line with respect to the main object
                            lineRenderer.enabled = true;
                            lineRenderer.SetPosition(0, cameraPosition + currentActiveModel.transform.position);
                            lineRenderer.SetPosition(1, hitPosition + currentActiveModel.transform.position);

                        }
                        else {
                            lineRenderer.enabled = false;
                        }
                    }
                }
            }

        }

        // if the number of groups fell (for example from 4 to 2), then we should destroy extra laser objects
        // mainObjectContainers.Length = number of main objects aka number of lasers to render
        if (laserGameObjects.Count > mainObjectContainers.Length) {
            int numberOfLasersToDestroy = laserGameObjects.Count - mainObjectContainers.Length;

            // start from after the lasers we want to keep, destory all of the rest of the lasers
            laserGameObjects.RemoveRange(mainObjectContainers.Length, numberOfLasersToDestroy);
        }






        // update the group number text at the top of the control panel (for devices)

        // if device (not instructor)
        if (!isInstructorGUIToggle) {

            // get the text component from groupNumberTextObject
            TextMeshProUGUI textbox = groupNumberTextObject.GetComponent<TextMeshProUGUI>();

            // write the current group number using currentUserGroupNumber
            textbox.text = "Group Number: " + currentUserGroupNumber;
            
        }


    }

    // these functions are to be called by GUIManager every time the device user switches between student, admin, and camera modes
    public void SetDeviceModeAdmin() {
        deviceCurrentMode = DeviceModes.Admin;
    }
    public void SetDeviceModeStudent() {
        deviceCurrentMode = DeviceModes.Student;
    }
    public void SetDeviceModeCamera() {
        deviceCurrentMode = DeviceModes.Camera;
    }
    



    // this function is now redundant because of OnCreateWallsPressed()
    // public void OnAdminEnableSceneManager() {
    //     SampleController.Instance.Log("AdminEnableSceneManager Pressed\nenabling OVRSceneManagerObj");
    //     OVRSceneManagerObj.SetActive(true);
    //     // OVRSceneManagerObj.SetActive(!OVRSceneManagerObj.activeSelf);
    //     SampleController.Instance.Log("Scene manager state: " + OVRSceneManagerObj.activeSelf);

    // }

    public void OnCreateWallsPressed() {
        SampleController.Instance.Log("OnCreateWallsPressed");

        TheaterModeManager.Instance.CreateScenePlaneClones();

    }
    
    public void OnDestroyWallsPressed() {
        SampleController.Instance.Log("OnDestroyWallsPressed");

        TheaterModeManager.Instance.DestroyScenePlaneClones();

    }




    
    private void InstantiateAlignedTable(Vector3 bottomLeft, Vector3 bottomRight, Vector3 topRight) {

        GameObject tableObject = PhotonPun.PhotonNetwork.Instantiate(alignedTablePrefab.name, new Vector3(0,0,0), Quaternion.identity);
        // Bounds tableBounds = tableObject.GetComponent<MeshFilter>().mesh.bounds;
        // mesh bounds are in local space, and renderer bounds are in global space
        Bounds tableBounds = tableObject.GetComponent<MeshRenderer>().bounds;

        

        // adjust table y scale
        // goal table height: obj1.transform.position.y
        // current table height: tableHeight
        // scale up factor needed: obj1.transform.position.y / tableHeight
        float tableHeight = tableBounds.size.y;

        // float yScaleUpFactor = bottomLeft.y / tableHeight;
        // instead of making the table go to the ground, hardcode the table height and make it floating
        float yScaleUpFactor = 0.08f / tableHeight;

        tableObject.transform.localScale = new Vector3(
            tableObject.transform.localScale.x,
            tableObject.transform.localScale.y * yScaleUpFactor,
            tableObject.transform.localScale.z
        );

        // get the bounds again because they were updated because we scaled up the table
        tableBounds = tableObject.GetComponent<MeshRenderer>().bounds; 

        // adjust table y level so that the position of the table is set to half way to the top

        // the top of the table should be at bottomLeft.y
        // the new height of the table is tableBounds.size.y
        // therefore, place the table at bottomLeft.y - (tableBounds.size.y / 2)
        // also add an offset to the y value because the controller cannot go through the table when placing the points
        // new Vector3(0, 0.04f, 0)
        tableObject.transform.position = new Vector3(0, bottomLeft.y - (tableBounds.size.y / 2) - 0.04f, 0);

        // old code:
        // tableObject.transform.position.y -= tableBounds.min.y;
        // tableObject.transform.position -= new Vector3(0, tableBounds.min.y, 0);
        // tableObject.transform.position = new Vector3(0, -tableBounds.min.y, 0);



        // get the bounds again because they were updated because we scaled up the table
        tableBounds = tableObject.GetComponent<MeshRenderer>().bounds; 

        // adjust table x,z position

        // find table center position using bottomLeft and topRight diagonal's center
        Vector3 diagonalMiddlePosition = (bottomLeft + topRight) / 2;

        // goal "center" position is diagonalMiddlePosition.x, current "center" position tableBounds.center.x, 
        // so we should add diagonalMiddlePosition.x - tableBounds.center.x to shift it over

        tableObject.transform.position += new Vector3(
            diagonalMiddlePosition.x - tableBounds.center.x, 
            0, 
            diagonalMiddlePosition.z - tableBounds.center.z
        );

        // get the bounds again because they were updated because we scaled up the table
        tableBounds = tableObject.GetComponent<MeshRenderer>().bounds; 



        // scale the table x z up so that the width and length match the distances between the points
        // distance between points = 
        float pointsDistanceX = Vector3.Distance(bottomLeft, bottomRight);
        float pointsDistanceZ = Vector3.Distance(bottomRight, topRight);

        float tableOriginalX = tableBounds.size.x;
        float tableOriginalZ = tableBounds.size.z;

        // scale up = distance / original length
        float xScaleUpFactor = pointsDistanceX / tableOriginalX;
        float zScaleUpFactor = pointsDistanceZ / tableOriginalZ;
        
        tableObject.transform.localScale = new Vector3(
            tableObject.transform.localScale.x * xScaleUpFactor,
            tableObject.transform.localScale.y,
            tableObject.transform.localScale.z * zScaleUpFactor
        );


        // before doing the rotations, create SeatMarker child objects for grouping purposes
        int rowNumber = nextDeskRowNumber;
        var seatsInThisRow = rowNumber switch
        {
            1 => 6,
            2 => 9,
            3 => 11,
            _ => 15,
        };
        nextDeskRowNumber++;

        // get the bounds again because they were updated because we scaled up the table
        tableBounds = tableObject.GetComponent<MeshRenderer>().bounds; 

        // create evenly spaced markers along the length of the desk, shifted back to esimate the position of the seats
        for (int i = 1; i <= seatsInThisRow; i++) {
            // length of the table is in x direction
            float tableLength = tableBounds.size.x;
            float markerX = tableBounds.min.x + i * (tableLength / (seatsInThisRow+1));

            float tableWidth = tableBounds.size.z;
            // place seat marker behind the desk
            // float markerZ = tableBounds.center.z - tableWidth;
            float markerZ = tableBounds.center.z + tableWidth; // plus because I assume the admin is facing the back of the room when creating the table

            float markerY = tableBounds.center.y;

            GameObject newMarkerObject = PhotonNetwork.Instantiate(seatMarkerPrefab.name, new Vector3(markerX, markerY, markerZ), Quaternion.identity);
            newMarkerObject.transform.SetParent(tableObject.transform);
            // set the parent to the table object before rotation so that,
            // after rotation, the marker positions will remain with respect to the desk.

            // when instantiating the seat markers, set photon custom properties 
            // MarkerDataStruct value = new();
            int[] value = new int[3];
            // value.row = rowNumber;
            // value.column = i;
            // value.totalSeatsInThisRow = seatsInThisRow;
            value[0] = rowNumber;
            value[1] = i;
            value[2] = seatsInThisRow;
            
            string key = "marker" + newMarkerObject.GetPhotonView().ViewID;
            SetRoomCustomProperty(key, value);

            // enable the visual component of the marker for debugging purposes
            newMarkerObject.transform.GetChild(0).gameObject.SetActive(true);
        }


        
        // now that we have created the markers with respect to the bounds, we can rotate to lose
        // the guarantee that the table is axis aligned

        // rotate the table so its corners can be in the same axes as the points
        // direction of table forward can be obtained using the first 2 points cross product with up direction
        Vector3 tableForwardDirection = Vector3.Cross(bottomRight - bottomLeft, Vector3.up).normalized;
        var headingChange = Quaternion.FromToRotation(tableObject.transform.forward, tableForwardDirection);
        tableObject.transform.localRotation *= headingChange;



        
    }


    public void OnDestroyAndResetAlignedTablesPressed() {
        SampleController.Instance.Log("OnDestroyAndResetAlignedTablesPressed");
        DestroyAndResetAlignedTables();
    }

    private void DestroyAndResetAlignedTables() {
        // reset next row number
        nextDeskRowNumber = 1;

        // get all desks
        GameObject[] alignedTableObjects = GameObject.FindGameObjectsWithTag("AlignedTable");

        // delete all desks and spatial anchors
        foreach (GameObject alignedTable in alignedTableObjects) {
            alignedTable.GetComponent<AlignedTable>().DestroyThisAndAnchor();
        }

        // delete all markers
        GameObject[] seatMarkerObjects = GameObject.FindGameObjectsWithTag("SeatMarker");
        foreach (GameObject seatMarker in seatMarkerObjects) {
            // Destroy(seatMarkers);
            PhotonNetwork.Destroy(seatMarker); // cloud object; call cloud destroy
        }

    }








    public void OnSpawnSphereButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnSphereButtonPressed");

        SpawnSphere();
    }

    private void SpawnSphere()
    {
        var sphereObject = PhotonPun.PhotonNetwork.Instantiate(spherePrefab.name, spawnPoint.position, spawnPoint.rotation);
        var photonGrabbable = sphereObject.GetComponent<PhotonGrabbableObject>();
        photonGrabbable.TransferOwnershipToLocalPlayer();


        int currentUserGroupNumber = GetCurrentGroupNumber();

        // print the sphere's group number if it has an "object data" property
        // ObjectData data = sphereObject.GetComponent<ObjectData>();
        // data.SetGroupNumber(currentUserGroupNumber); // set the group number to current user's group number
        // SampleController.Instance.Log("group number of this sphere is: " + data.groupNumber);

        // we will actually store the group number in the room custom properties
        // use the view id of the sphere

        SetPhotonObjectGroupNumber(sphereObject, currentUserGroupNumber);

        // string key = "groupNum" + sphereObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        // int value = currentUserGroupNumber;
        // var newValue = new ExitGames.Client.Photon.Hashtable { { key, value } };
        // PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newValue);

        // the custom properties table is not set in time
        // SampleController.Instance.Log("group number of this sphere is: " + PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key]);
        SampleController.Instance.Log("setting group number of sphere to: " + currentUserGroupNumber);

        mostRecentSphere = sphereObject;
    }


    private void SetPhotonObjectGroupNumber(GameObject photonObject, int groupNumber) {

        string key = "groupNum" + photonObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        int value = groupNumber;
        var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomProperty);

    }

    private bool PhotonObjectHasGroupNumber(GameObject photonObject) {

        string key = "groupNum" + photonObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }

    private int GetPhotonObjectGroupNumber(GameObject photonObject) {

        string key = "groupNum" + photonObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        int objectGroupNumber = (int)PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];

        return objectGroupNumber;
    }


    // room has custom property?
    private bool RoomHasCustomProperty(string key) {
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }

    // get room custom property
    private object GetRoomCustomProperty(string key) {

        // string key = "groupNum" + photonObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        // int objectGroupNumber = (int)PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];

        // return objectGroupNumber;
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];
    }


    private void SetRoomCustomProperty(string key, object value) {

        var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
        PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomProperty);

    }




    public void OnSpawnJengaButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnJengaButtonPressed");

        SpawnJenga();
    }

    private void SpawnJenga()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(jengaPrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();
    }

    // public void OnSpawnTableButtonPressed()
    // {
    //     SampleController.Instance.Log("OnSpawnTableButtonPressed");

    //     SpawnTable();
    // }

    // private void SpawnTable()
    // {
    //     var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
    //     // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
    //     // photonGrabbable.TransferOwnershipToLocalPlayer();
    // }



    public void OnCreateNewAlignedTableButtonPressed()
    {
        SampleController.Instance.Log("OnCreateNewAlignedTableButtonPressed");

        CreateNewAlignedTable();
    }

    private void CreateNewAlignedTable()
    {
        alignTableMode = true;
    }



    public void OnSetGroupNumber(int groupNumber) {
        SampleController.Instance.Log("Setting group number to " + groupNumber);
        SetGroupNumber(groupNumber);
    }
    private void SetGroupNumber(int groupNumber) {
        // gameObject.GetComponent<StudentData>().SetGroupNumber(groupNumber);
        // SampleController.Instance.Log("Set group number to " + gameObject.GetComponent<StudentData>().groupNumber);

        PhotonRealtime.Player LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;

        LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });

        // also set locally for faster updates
        LocalPlayer.CustomProperties["groupNumber"] = groupNumber;


    }



    public void OnIncrementGroupNumber() {
        int newGroupNumber = GetCurrentGroupNumber() + 1;
        SampleController.Instance.Log("incrementing group number to " + newGroupNumber);
        SetGroupNumber(newGroupNumber);
    }

    public void OnDecrementGroupNumber() {
        int newGroupNumber = GetCurrentGroupNumber() - 1;
        SampleController.Instance.Log("decrementing group number to " + newGroupNumber);
        SetGroupNumber(newGroupNumber);
    }

    private int GetCurrentGroupNumber() {
        if(isInstructorGUIToggle) return 0;

        ExitGames.Client.Photon.Hashtable PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int currentUserGroupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : defaultGroupNumber;

        return currentUserGroupNumber;

    }

    private int GetPlayerGroupNumber(PhotonRealtime.Player player) {

        ExitGames.Client.Photon.Hashtable PlayerProperties = player.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int groupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : defaultGroupNumber;

        return groupNumber;

    }

    public void OnLogCurrentGroupNumber() {
        SampleController.Instance.Log("Your group number is " + GetCurrentGroupNumber());
        
    }




    public void OnSetEveryoneElseGroupNumber(int groupNumber) {

        SampleController.Instance.Log("Setting everyone else's group number to " + groupNumber);

        SetEveryoneElseGroupNumber(groupNumber);

    }

    private void SetEveryoneElseGroupNumber(int groupNumber) {

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                SampleController.Instance.Log("(skipping current player)");
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            SampleController.Instance.Log("Set player group of nickname: " + player.NickName);
        }

    }



    public void OnSetToAdminMode() {
        SampleController.Instance.Log("Setting to Admin Mode");
        SetToAdminMode();
    }

    private void SetToAdminMode() {

        SampleController.Instance.Log("This buttons code was moved to GUIManager");


        // for every button in a list of buttons, make them active


        // foreach (GameObject b in studentButtons) {
        //     b.SetActive(false);
        // }
        // foreach (GameObject b in adminButtons) {
        //     b.SetActive(true);
        // }
        

    }

    public void OnSetToStudentMode() {
        SampleController.Instance.Log("Setting to Admin Mode");
        SetToStudentMode();
    }

    private void SetToStudentMode() {
        SampleController.Instance.Log("This buttons code was moved to GUIManager");

        // for every button in a list of buttons, make them active
        // foreach (GameObject b in adminButtons) {
        //     b.SetActive(false);
        // }
        // foreach (GameObject b in studentButtons) {
        //     b.SetActive(true);
        // }

    }


    public void OnSetEveryoneSeparateGroups() {
        SampleController.Instance.Log("Setting everyone to separate groups (except admin)");
        SetEveryoneSeparateGroups();
    }

    private void SetEveryoneSeparateGroups() {

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        int groupNumber = 1;
        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                SampleController.Instance.Log("(skipping current player)");
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            SampleController.Instance.Log("Set player group of nickname " + player.NickName + " to group " + groupNumber);
            groupNumber++;
        }

    }



    public void OnSetEveryoneGroupsOfTwo() {
        SampleController.Instance.Log("Setting everyone into groups of two (except admin)");
        SetEveryoneSeparateGroups();
    }

    private void SetEveryoneGroupsOfTwo() {

        // value collection (basically list) of PhotonRealtime.Player objects
        // values because players are like a dictionary (we dont want the keys)
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        int groupNumber = 1;
        int counter = 0;
        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                SampleController.Instance.Log("(skipping current player)");
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            SampleController.Instance.Log("Set player group of nickname " + player.NickName + " to group " + groupNumber);
            counter++;
            if (counter % 2 == 0) {
                groupNumber++;
            }
        }

    }




    public void OnCreateMainObjectContainerPerGroup() {
        SampleController.Instance.Log("Creating a Main Object Container Per Group (except group 0)");
        CreateMainObjectContainerPerGroup();
    }

    private void CreateMainObjectContainerPerGroup() {


        // value collection (basically list) of PhotonRealtime.Player objects
        // values because players are like a dictionary (we dont want the keys)
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        // get max group number
        int maxGroupNumber = 0; 

        foreach (PhotonRealtime.Player player in players) {
            int groupNumber = GetPlayerGroupNumber(player);
            if (maxGroupNumber < groupNumber) maxGroupNumber = groupNumber;
        }

        SampleController.Instance.Log("max group number is " + maxGroupNumber);

        // create headPositionsPerGroup array: stores list of player positions for each group
        // array of list ints
        List<Vector3>[] headPositionsPerGroup = new List<Vector3>[maxGroupNumber+1];
            // +1 because we should be able to access the array at the max group number index
        
        // initialize all of the lists to empty lists
        for (int i = 1; i <= maxGroupNumber; i++) {
            headPositionsPerGroup[i] = new List<Vector3>();
        }

        // get every MyPhotonUserHeadTracker 
        var allHeadTrackerObjects = FindObjectsByType<PhotonUserHeadTrackerCommunication>(FindObjectsSortMode.None);

        // populate the headPositionsPerGroup array
        foreach (PhotonUserHeadTrackerCommunication headTrackerScript in allHeadTrackerObjects) {
            // get the head tracker object
            GameObject headTrackerObject = headTrackerScript.gameObject;

            // get photon view
            var photonView = headTrackerObject.GetComponent<PhotonPun.PhotonView>();

            // get player owner 
            PhotonRealtime.Player player = photonView.Owner;

            // get player's group number
            int groupNumber = GetPlayerGroupNumber(player);
            // skip if group number is 0
            if (groupNumber == 0) {
                continue;
            }

            // get the vector 3 associated with this player's head
            Vector3 position;
            // if local head:
            if (photonView.IsMine) {
                position = UserHeadPositionTrackerManager.Instance.localHeadTransform.position;
            }
            else {
                // not local head
                position = headTrackerObject.transform.position;
            }

            // append to array
            headPositionsPerGroup[groupNumber].Add(position);
        }

        // now we have all of the vector3 in a list for each group (done)

        // for each group, get the average position of the group members
        // and instantiate a Main Object Container at that position
        for (int i = 1; i <= maxGroupNumber; i++) {
            List<Vector3> headPositions = headPositionsPerGroup[i];
            SampleController.Instance.Log("Group " + i + " has " + headPositions.Count + " head transforms"); 

            // if list is empty, then skip
            if (headPositions.Count == 0) {
                SampleController.Instance.Log("Skipping group " + i); 
                continue;
            }

            // get the average of all transforms of this group

            Vector3 averageVector = Vector3.zero;
            foreach(Vector3 position in headPositions) {
                averageVector += position;
            }
            averageVector /= headPositions.Count;

            SampleController.Instance.Log("average position spawn point is " + averageVector); 
            

            // now, instantiate the Main Object container at this position and for this group

            // instantiate
            var mainObjectContainerInstance = PhotonPun.PhotonNetwork.Instantiate(mainObjectContainerPrefab.name, averageVector, mainObjectContainerPrefab.transform.rotation);
            // set group number
            SetPhotonObjectGroupNumber(mainObjectContainerInstance, i);

        }

    }



    public void OnSetMainObjectModel1() {
        SampleController.Instance.Log("Setting main object to model x");
        SetMainObjectModel1();
    }

    private void SetMainObjectModel1() {

        SetRoomCustomProperty("mainObjectCurrentModelName", "Model1");

    }

    public void OnSetMainObjectModel2() {
        SampleController.Instance.Log("Setting main object to model 2");
        SetMainObjectModel2();
    }

    private void SetMainObjectModel2() {

        SetRoomCustomProperty("mainObjectCurrentModelName", "Model2");

    }



}