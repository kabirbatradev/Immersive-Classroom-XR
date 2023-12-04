
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
// using PlayerProperties = Photon.Pun.PhotonNetwork.CustomProperties;
// using PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;
// using LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;

public class SharedAnchorControlPanelAdditionalFunctions : MonoBehaviour
{


    [SerializeField]
    private bool isInstructorGUIToggle;





    [SerializeField]
    private GameObject spherePrefab;


    [SerializeField]
    private GameObject jengaPrefab;

    [SerializeField]
    private GameObject tablePrefab;


    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject mainObjectContainerPrefab;



    [SerializeField]
    private GameObject[] adminButtons;

    [SerializeField]
    private GameObject[] studentButtons;


    // private bool alignTableMode = false;
    // private int countAButton = 0;

    private GameObject mostRecentSphere;





    private LineRenderer lineRenderer;
    private float lineSize = 0.02f;
    public Material laserMaterial;


    public void Start() {
        // initialize laser renderer

        if (lineRenderer == null) {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = laserMaterial;
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;
        }

        lineRenderer.enabled = false;
    }


    public void Update() {


        if (PhotonPun.PhotonNetwork.CurrentRoom == null) {
            return;
        }

        /*
        if (alignTableMode) {

            // bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
            bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.A);

            
            if (buttonPressed) {
                SampleController.Instance.Log("The A button was pressed!");

                var controllerType = OVRInput.Controller.RTouch; // the right controller
                Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controllerType);

                string x = controllerPosition.x.ToString("0.00");
                string y = controllerPosition.y.ToString("0.00");
                string z = controllerPosition.z.ToString("0.00");
                SampleController.Instance.Log(x + " " + y + " " + z);

                countAButton++;
                if (countAButton == 2) {
                    countAButton = 0;
                    alignTableMode = false;
                }
            }
        }
        */


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

        
        int currentUserGroupNumber = GetCurrentGroupNumber(); // gets group number from local player's custom propreties

        // need to include inactive 
        // ObjectData[] allNetworkObjectDatas = (ObjectData[]) FindObjectsByType<ObjectData>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        PhotonPun.PhotonView[] allPhotonViews = (PhotonPun.PhotonView[]) FindObjectsByType<PhotonPun.PhotonView>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (PhotonPun.PhotonView photonView in allPhotonViews) {
            GameObject obj = photonView.gameObject;
            if (obj == null) {
                // SampleController.Instance.Log("photonView was attached to null object");
                Debug.Log("photonView was attached to null object");
                continue;
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
        // if you press X, then print all spawned object's data
        bool XPressed = OVRInput.GetDown(OVRInput.RawButton.X);
        if (XPressed) {
            foreach (ObjectData objectData in allNetworkObjectDatas) {
                GameObject obj = objectData.gameObject;
                // SampleController.Instance.Log("instance id: " + obj.GetInstanceID().ToString());
                SampleController.Instance.Log("object view id: " + obj.GetComponent<PhotonPun.PhotonView>().ViewID.ToString());


                // string key = "groupNum" + obj.GetComponent<PhotonPun.PhotonView>().ViewID;
                // int objectGroupNumber = (int)PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];

                SampleController.Instance.Log("object group number: " + GetPhotonObjectGroupNumber(obj));
                SampleController.Instance.Log("");
            }
        }
        */



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
                // Debug.Log("AJDFGKJHLASDFL;KJASD;FJKLHBASDGFKJASBDF;KH");


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

        
        // // test if line renderer is working (works)
        // lineRenderer.enabled = true;
        // lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
        // lineRenderer.SetPosition(1, new Vector3(1, 1, 1));


    }

    // testing purposes
    // private bool isShootingExisted = false;

    





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

    public void OnSpawnTableButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnTableButtonPressed");

        SpawnTable();
    }

    private void SpawnTable()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();
    }



    public void OnSpawnAlignedTableButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnAlignedTableButtonPressed");

        SpawnAlignedTable();
    }

    private void SpawnAlignedTable()
    {

        // var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();



        // alignTableMode = true;
        // SampleController.Instance.Log(alignTableMode.ToString());
        
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


    }



    private int GetCurrentGroupNumber() {

        ExitGames.Client.Photon.Hashtable PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int currentUserGroupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : 0;

        return currentUserGroupNumber;

    }

    private int GetPlayerGroupNumber(PhotonRealtime.Player player) {

        ExitGames.Client.Photon.Hashtable PlayerProperties = player.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int groupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : 0;

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

        // for every button in a list of buttons, make them active


        foreach (GameObject b in studentButtons) {
            b.SetActive(false);
        }
        foreach (GameObject b in adminButtons) {
            b.SetActive(true);
        }
        

    }

    public void OnSetToStudentMode() {
        SampleController.Instance.Log("Setting to Admin Mode");
        SetToStudentMode();
    }

    private void SetToStudentMode() {

        // for every button in a list of buttons, make them active
        foreach (GameObject b in adminButtons) {
            b.SetActive(false);
        }
        foreach (GameObject b in studentButtons) {
            b.SetActive(true);
        }

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