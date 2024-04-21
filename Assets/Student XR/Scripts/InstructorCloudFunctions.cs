using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class InstructorCloudFunctions : MonoBehaviour
{

    public static int defaultGroupNumber = 1;

    public static InstructorCloudFunctions Instance;

    private void Awake() {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }




    [SerializeField]
    private GameObject mainObjectContainerPrefab;


    [SerializeField]
    private GameObject panelPrefab;

    [SerializeField]
    private bool debugMode = false;

    // we need room custom properties for laser length so other scripts can use that laser length, but we dont need it for the group mode
    // public const string groupModeKey = "GroupMode";

    // small groups is usually groups of 4
    enum GroupMode {LargeLectureMode, IndividualMode, SmallGroupsMode, Fallback};
    private GroupMode currentGroupMode = GroupMode.Fallback;
    // private GroupMode currentGroupMode = GroupMode.LargeLectureMode;
    public const string laserLengthKey = "LaserLength";
    private const float individualModeLaserLength = 0.5f;
    private const float largeLectureModeLaserLength = 4.0f;
    private const float smallGroupsModeLaserLength = 1.0f;

    private const int largeLectureModeMainObjectScale = 5;
    private const float individualModeObjectScale = 1;
    private const float smallGroupsModeObjectScale = 1.5f;

    void Update() {
        if (debugMode) {
            if (Input.GetKeyDown(KeyCode.P)) {
                // p for spawning panels
                CreatePanelPerGroup();
            }

            if (Input.GetKeyDown(KeyCode.X)) {
                // p for spawning panels
                // DestroyAllPanels();
            }
        }

    }
    


    public List<Player> GetAllPlayers() {

        // dictionary values
        var players = PhotonNetwork.CurrentRoom.Players.Values;
        return players.ToList();
    }

    public bool PlayerIsStudent(Player player) {
        // local player = instructor; group number 0 = admin
        return !(
            player.Equals(PhotonNetwork.LocalPlayer) || 
            GetPlayerGroupNumber(player) == 0
        );
    }

    // filtered players: no admin or instructor
    public List<Player> GetAllStudents() {
        var players = GetAllPlayers();
        return players.Where(p => PlayerIsStudent(p)).ToList();
    }



    // get player group number from player custom properties
    public int GetPlayerGroupNumber(PhotonRealtime.Player player) {

        bool groupNumberExists = player.CustomProperties.ContainsKey("groupNumber");
        int groupNumber = groupNumberExists ? (int)player.CustomProperties["groupNumber"] : defaultGroupNumber;

        return groupNumber;

    }


    public int GetMaxGroupNumber() {
        if (PhotonNetwork.CurrentRoom == null) return 0;

        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        // get max group number
        int maxGroupNumber = 0; 

        foreach (PhotonRealtime.Player player in players) {
            int groupNumber = GetPlayerGroupNumber(player);
            if (maxGroupNumber < groupNumber) maxGroupNumber = groupNumber;
        }

        return maxGroupNumber;
    }


    // call the correct "create main object" function depending on which group mode is currently set
    public void CreateMainObjectContainerPerGroup() {
        
        // get the custom property group mode 
        // bool groupModeIsSet = RoomHasCustomProperty(groupModeKey);
        // GroupMode mode = currentGroupMode;

        if (currentGroupMode == GroupMode.Fallback) {
            Debug.Log("group mode not set; calling large lecture mode");
            LargeLectureMode();
        }

        // string groupMode = (string)GetRoomCustomProperty(groupModeKey);
        if (currentGroupMode == GroupMode.LargeLectureMode) {
            CreateMainObjectsForLectureMode();
        }
        else if (currentGroupMode == GroupMode.IndividualMode) {
            // not implemented yet, go to fallback
            // Debug.Log("RecreateMainObjectsIfTheyExist: fallback group mode, calling OLDCreateMainObjectContainerPerGroup");
            CreateMainObjectsForIndividualMode();
        }
        else if (currentGroupMode == GroupMode.SmallGroupsMode) {
            // Debug.Log("RecreateMainObjectsIfTheyExist: fallback group mode, calling OLDCreateMainObjectContainerPerGroup");
            CreateMainObjectsForSmallGroupsMode();
        }
        else {
            // i got rid of fallback so this shouldnt happen, it will just be lecture mode
            // fallback
            Debug.Log("RecreateMainObjectsIfTheyExist: fallback group mode, calling OLDCreateMainObjectContainerPerGroup");
            OLDCreateMainObjectContainerPerGroup();
        }

    }
    

    // places main objectat the average position of all the students for each group
    public void OLDCreateMainObjectContainerPerGroup() {


        // before creating new main objects, delete any preexisting objects
        DeleteAllMainObjects();


        // // get all players:
        // // value collection (basically list) of PhotonRealtime.Player objects
        // // values because players are like a dictionary (we dont want the keys)
        // var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        // get max group number
        int maxGroupNumber = GetMaxGroupNumber(); 

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
            // skip if group number is 0 (admin)
            if (groupNumber == 0) {
                continue;
            }

            // get the vector 3 associated with this player's head
            Vector3 position;
            // if local head: (this only happens if this function was called by a admin headset and not a laptop)
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

            // if list is empty, then skip
            if (headPositions.Count == 0) {
                continue;
            }

            // get the average of all transforms of this group

            Vector3 averageVector = Vector3.zero;
            foreach(Vector3 position in headPositions) {
                averageVector += position;
            }
            averageVector /= headPositions.Count;



            // for groups of 4, we should not shift by 1 z
            // adjust this averageVector spawn point by an offset: instantiate the object in front of the group
            averageVector += new Vector3(0, 0, 1);



            // now, instantiate the Main Object container at this position and for this group

            // instantiate
            var mainObjectContainerInstance = PhotonPun.PhotonNetwork.Instantiate(mainObjectContainerPrefab.name, averageVector, mainObjectContainerPrefab.transform.rotation);
            // set group number
            SetPhotonObjectGroupNumber(mainObjectContainerInstance, i);

        }



    }




    public void DeleteAllMainObjects() {
        // get all main objects
        GameObject[] mainObjects = GameObject.FindGameObjectsWithTag("MainObjectContainer");
        int count = mainObjects.Length;
        foreach (var mainObject in mainObjects) {

            // will fail to destroy object if not the owner of the object

            // therefore, transfer ownership to local player (the instructor), and then destroy
            PhotonPun.PhotonView photonView = mainObject.GetComponent<PhotonPun.PhotonView>();
            photonView.TransferOwnership(Photon.Pun.PhotonNetwork.LocalPlayer); 
            PhotonPun.PhotonNetwork.Destroy(mainObject);
        }
        Debug.Log("removed " + count + " main objects");
    }



    private bool MainObjectsExist() {
        GameObject[] mainObjects = GameObject.FindGameObjectsWithTag("MainObjectContainer");
        return mainObjects.Length > 0;
    }


    private void RecreateMainObjectsIfTheyExist() {

        if (!MainObjectsExist()) {
            return;
        }

        CreateMainObjectContainerPerGroup();

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





    public void IndividualMode() {

        
        // SetRoomCustomProperty(groupModeKey, "IndividualMode");
        currentGroupMode = GroupMode.IndividualMode;
        SetRoomCustomProperty(laserLengthKey, individualModeLaserLength);

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        int groupNumber = 1;
        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                Debug.Log("(skipping current player)");
                continue;
            }
            // skip players of group number 0 (admins)
            if (GetPlayerGroupNumber(player) == 0) {
                Debug.Log("skipping player with group number 0: " + player.NickName);
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            // also set the local cache so recreating main object uses correct group number even if cloud hasnt updated yet
            player.CustomProperties["groupNumber"] = groupNumber;
            Debug.Log("Set player group of nickname " + player.NickName + " to group " + groupNumber);
            groupNumber++;
        }

        RecreateMainObjectsIfTheyExist();
        DestroyAllPanels();


    }

    public void SetStudentsIntoIndividualGroups() {
        IndividualMode();
    }

    public void CreateMainObjectsForIndividualMode() {
        
        Debug.Log("Individual Mode create main objects called");

        // before creating new main objects, delete any preexisting objects
        DeleteAllMainObjects();

        // for individual mode, create a new object per player, and set the group number respectively
        

        GameObject[] playerHeadObjects = GameObject.FindGameObjectsWithTag("PlayerHead");
        foreach (GameObject playerHeadObj in playerHeadObjects) {
            PhotonView photonView = playerHeadObj.GetPhotonView();
            Player player = photonView.Owner;

            int groupNumber = GetPlayerGroupNumber(player);

            // skip if group number is 0 or if instructor (local)
            if (groupNumber == 0 || PhotonNetwork.LocalPlayer == player) {
                continue;
            }

            // get the vector 3 associated with this player's head
            Vector3 mainObjectSpawnPosition;
            // if local head: (this only happens if this function was called by a admin headset and not a laptop)
            if (photonView.IsMine) {
                mainObjectSpawnPosition = UserHeadPositionTrackerManager.Instance.localHeadTransform.position;
            }
            else {
                // not local head
                mainObjectSpawnPosition = playerHeadObj.transform.position;
            }

            // create the main object for this player:

            // shift the spawn position forward by 1 meter (in front of the player); actually 0.5 might be better
            mainObjectSpawnPosition += new Vector3(0, 0, 1.0f);
            var mainObjectContainerInstance = PhotonNetwork.Instantiate(mainObjectContainerPrefab.name, mainObjectSpawnPosition, mainObjectContainerPrefab.transform.rotation);
            // set group number
            SetPhotonObjectGroupNumber(mainObjectContainerInstance, groupNumber);

            // set scale
            foreach (Transform mainObjectTransform in mainObjectContainerInstance.transform) {
                mainObjectTransform.localScale = new Vector3(individualModeObjectScale, individualModeObjectScale, individualModeObjectScale);
            }
        }

    }




    // dont plan to use this in the actual demo
    public void SetStudentsIntoGroupsOfTwo() {

        // value collection (basically list) of PhotonRealtime.Player objects
        // values because players are like a dictionary (we dont want the keys)
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        int groupNumber = 1;
        int counter = 0;
        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                Debug.Log("(skipping current player)");
                continue;
            }
            // skip players of group number 0 (admins)
            if (GetPlayerGroupNumber(player) == 0) {
                Debug.Log("skipping player with group number 0: " + player.NickName);
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            // also set the local cache so recreating main object uses correct group number even if cloud hasnt updated yet
            player.CustomProperties["groupNumber"] = groupNumber;
            SampleController.Instance.Log("Set player group of nickname " + player.NickName + " to group " + groupNumber);
            counter++;
            if (counter % 2 == 0) {
                groupNumber++;
            }
        }

        RecreateMainObjectsIfTheyExist();
        DestroyAllPanels();

    }



    // all students are in one group, main object is at the front of the room and very large, long laser length
    public void LargeLectureMode() {

        Debug.Log("Large Lecture Mode called");

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        foreach (PhotonRealtime.Player player in players) {
            // does nothing for admins and instructor
            SetPlayerGroupNumber(player, 1);
        }

        // set room custom property of the current mode
        currentGroupMode = GroupMode.LargeLectureMode;
        // SetRoomCustomProperty(groupModeKey, "LargeLectureMode");

        // set laser length
        SetRoomCustomProperty(laserLengthKey, largeLectureModeLaserLength);

        // recreate main objects if they exist
        RecreateMainObjectsIfTheyExist();
        DestroyAllPanels();
        
    }

    // calls the LectureMode() function; only keeping this so the preexisting instructor button doesnt break
    public void SetAllStudentsGroupOne() {
        LargeLectureMode();
    }

    // there is only one group, and the position of the main object is fixed: at the front of the room
    public void CreateMainObjectsForLectureMode() {

        Debug.Log("CreateMainObjectsForLectureMode");

        // before creating new main objects, delete any preexisting objects
        DeleteAllMainObjects();


        bool deskFound = GameObject.FindWithTag("AlignedTable") != null;

        MinMax boundingBox = new();

        // if desks exist, create bounding box around all desks and identify positon of front middle of desks
        if (deskFound) {

            GameObject[] tableObjects = GameObject.FindGameObjectsWithTag("AlignedTable");
            foreach (var table in tableObjects) {
                boundingBox.AddPoint(table.transform.position);
            }

        }
        // if desks dont exist, get bounding box of all students, get position in front
        else {
            // get PlayerHead game objects instead of the players list so we know where they are too
            GameObject[] playerHeadObjects = GameObject.FindGameObjectsWithTag("PlayerHead");
            // foreach (Player player in GetAllStudents()) {
            foreach (GameObject playerHeadObject in playerHeadObjects) {
                Player player = GetPlayerFromPlayerHeadObject(playerHeadObject);

                if (!PlayerIsStudent(player)) {
                    continue;
                }
                boundingBox.AddPoint(playerHeadObject.transform.position);
            }
        }

        // get front middle position of bounding box
        Vector3 max = boundingBox.max;
        Vector3 min = boundingBox.min;
        Vector3 center = (min + max) / 2;
        
        // x should be middle of bounding box
        // y should be min of bounding box (lowest student head or desk is first row)
        // z should be max of bounding box (front of the room)
        Vector3 frontMostDeskPosition = new Vector3(center.x, min.y, max.z);

        // place main object in front and above this front desk position (few meters up, few meters forward)
        // 2.5 up is too high; reduce that to 1.75 or 2
        Vector3 mainObjectPosition = new Vector3(0, 2.0f, 2) + frontMostDeskPosition;
        var mainObjectContainer = PhotonNetwork.Instantiate(mainObjectContainerPrefab.name, mainObjectPosition, mainObjectContainerPrefab.transform.rotation);
        
        // set group number of main object
        SetPhotonObjectGroupNumber(mainObjectContainer, 1);

        // set the scale of the object: must set the scale of all subobjects instead of the main object 
        // container because this is what the instructor object streams through custom properties in CommunicationScript

        foreach (Transform mainObjectTransform in mainObjectContainer.transform) {
            mainObjectTransform.localScale = new Vector3(largeLectureModeMainObjectScale, largeLectureModeMainObjectScale, largeLectureModeMainObjectScale);
        }

    }














    // room has custom property?
    public bool RoomHasCustomProperty(string key) {
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key);
    }

    // get room custom property
    public object GetRoomCustomProperty(string key) {

        // string key = "groupNum" + photonObject.GetComponent<PhotonPun.PhotonView>().ViewID;
        // int objectGroupNumber = (int)PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];

        // return objectGroupNumber;
        return PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key];
    }


    public void SetRoomCustomProperty(string key, object value) {

        var newCustomProperty = new ExitGames.Client.Photon.Hashtable { { key, value } };
        // update on server
        PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newCustomProperty);

        // update locally because server will update local cached hashmap with delay
        PhotonPun.PhotonNetwork.CurrentRoom.CustomProperties[key] = value;

    }




    public void SetActiveModelNumber(int modelNumber) {
        // SetRoomCustomProperty("mainObjectCurrentModelName", "Model1");
        SetRoomCustomProperty("mainObjectCurrentModelName", "Model" + modelNumber);
    }

    public int getTotalNumberOfModels() {
        if (RoomHasCustomProperty("totalNumberOfModels")) {
            return (int)GetRoomCustomProperty("totalNumberOfModels");
        }
        else {
            return 2; // assume there are at least 2 models
        }
        
    }


    /// <summary>
    /// set single player's group number; 
    /// skips admins and local player automatically
    /// </summary>
    private void SetPlayerGroupNumber(Player player, int groupNumber) {

        // if the player is the current player (the instructor), then skip
        if (player.Equals(PhotonNetwork.LocalPlayer)) {
            Debug.Log("(not setting the localplayer/instructor's group number)");
            return;
        }

        // skip players of group number 0 (admins and camera mode)
        if (GetPlayerGroupNumber(player) == 0) {
            Debug.Log("skipping player with group number 0: " + player.NickName);
            return;
        }


        // set the player group number
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
        // also set the local cache so recreating main object uses correct group number even if cloud hasnt updated yet
        player.CustomProperties["groupNumber"] = groupNumber;
        Debug.Log($"Set player {player.NickName} to group number {groupNumber}");

    }



    
    // helper function for getting player object from player head game object
    private Player GetPlayerFromPlayerHeadObject(GameObject playerHead) {
        if (!playerHead.CompareTag("PlayerHead")) {
            Debug.Log("Error: This object is not a player head object (does not have player head tag)");
            return null;
        }
        
        return playerHead.GetPhotonView().Owner;
    }


    // sets a specific group number value for each student
    // automatically skips admins and local player
    // also automatically creates new main objects if they should currently be displayed
    // helper function to be used in AssignEachPlayerHeadToSpecificGroupNumber
    public void AssignEachStudentToSpecificGroupNumber(Player[] playerArray, int[] groupNumbers) {

        SmallGroupsMode(playerArray, groupNumbers);

    }

    public void SmallGroupsMode(Player[] playerArray, int[] groupNumbers) {

        // set current group mode and laser length
        currentGroupMode = GroupMode.SmallGroupsMode;
        SetRoomCustomProperty(laserLengthKey, smallGroupsModeLaserLength);


        if (playerArray.Length != groupNumbers.Length) {
            Debug.Log("Error: playerArray and groupNumbers length mismatch");
            return;
        }
        for (int i = 0; i < playerArray.Length; i++) {
            Player player = playerArray[i];
            int groupNumber = groupNumbers[i];

            if (groupNumber == 0) {
                groupNumber = 999;
                Debug.Log("Error: attempted to set group number to 0; setting to 999 as error message");
            }

            // skips admin and local player (instructor) automatically
            SetPlayerGroupNumber(player, groupNumber);
        }
        RecreateMainObjectsIfTheyExist();
        DestroyAllPanels();
        CreatePanelPerGroup(); // in this case we do need panels per group

    }






    // usage: 
    // GameObject[] FindGameObjectsWithTag(string tag) where tag = "PlayerHead"
    // use the transform of this object to determine which group number it should be assigned to
    // call this function, passing in the game object array and an array of group numbers
    // use InstructorCloudFunctions.Instance.AssignEachPlayerHeadToSpecificGroupNumber(..) to access this function globally

    // sets a specific group number value for each student
    // automatically skips admins and local player
    // also automatically creates new main objects if they should currently be displayed
    public void AssignEachPlayerHeadToSpecificGroupNumber(GameObject[] playerHeadArray, int[] groupNumbers) {
        Player[] playerArray = new Player[playerHeadArray.Length];
        for (int i = 0; i < playerHeadArray.Length; i++) {
            GameObject playerHead = playerHeadArray[i];
            playerArray[i] = GetPlayerFromPlayerHeadObject(playerHead);
            if (playerArray[i] == null) {
                Debug.Log("Since one of the player head objects is does not have the PlayerHead tag, we will not assign group numbers");
                return;
            }
        }     

        AssignEachStudentToSpecificGroupNumber(playerArray, groupNumbers);
    }



    public void CreateMainObjectsForSmallGroupsMode() {

        DeleteAllMainObjects();

        // get max group number
        int maxGroupNumber = GetMaxGroupNumber(); 

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
            // skip if group number is 0 (admin)
            if (groupNumber == 0) {
                continue;
            }

            // get the vector 3 associated with this player's head
            Vector3 position;
            // if local head: (this only happens if this function was called by a admin headset and not a laptop)
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

            // if list is empty, then skip
            if (headPositions.Count == 0) {
                continue;
            }

            // get the average of all transforms of this group

            // Vector3 averageVector = Vector3.zero;
            // foreach(Vector3 position in headPositions) {
            //     averageVector += position;
            // }
            // averageVector /= headPositions.Count;

            MinMax boundingBox = new();
            foreach(Vector3 position in headPositions) {
                boundingBox.AddPoint(position);
            }
            // instead of using average position of students, use the center of the bounding box
            Vector3 max = boundingBox.max;
            Vector3 min = boundingBox.min;
            Vector3 center = (min + max) / 2;

            // Vector3 panelPos = new Vector3(max.x + 1, center.y+0.5f, center.z); // this is where the panel is (to the right, up a bit)
            Vector3 mainObjectPosition = new Vector3(center.x, center.y, center.z);


            // for groups of 4, we should not shift by 1 z
            // adjust this averageVector spawn point by an offset: instantiate the object in front of the group
            // averageVector += new Vector3(0, 0, 1);
            // dont shift the position for the small groups mode; instead, place it in the middle of everyone


            // now, instantiate the Main Object container at this position and for this group

            // instantiate
            var mainObjectContainerInstance = PhotonPun.PhotonNetwork.Instantiate(mainObjectContainerPrefab.name, mainObjectPosition, mainObjectContainerPrefab.transform.rotation);
            // set group number
            SetPhotonObjectGroupNumber(mainObjectContainerInstance, i);

            // set the scale
            foreach (Transform mainObjectTransform in mainObjectContainerInstance.transform) {
                mainObjectTransform.localScale = new Vector3(smallGroupsModeObjectScale, smallGroupsModeObjectScale, smallGroupsModeObjectScale);
            }

        }

    }










    // basically an AABB (axis aligned bounding box)
    private class MinMax {
        public List<Vector3> points;
        public Vector3 min;
        public Vector3 max;

        // since its a class and not a struct, we can have a constructor function
        public MinMax() {
            points = new();
        }

        public void AddPoint(Vector3 point) {

            Debug.Log("AddPoint was called");

            // if this is the first point, set min and max
            if (points.Count == 0) {
                Debug.Log("points count was 0, setting min and max");
                points.Add(point);

                min = point; max = point;

                return;
            }

            // otherwise simply add the point and update min and max
            points.Add(point);

            // update min and max of bounding box
            min.x = Mathf.Min(point.x, min.x);
            min.y = Mathf.Min(point.y, min.y);
            min.z = Mathf.Min(point.z, min.z);

            max.x = Mathf.Max(point.x, max.x);
            max.y = Mathf.Max(point.y, max.y);
            max.z = Mathf.Max(point.z, max.z);
        }
    }

    
    public void CreatePanelPerGroup() {
        Debug.Log("CreatePanelPerGroup called");
        if (panelPrefab == null) {
            Debug.Log("ERROR: panel prefab is not set in InstructorCloudFunctions");
        }


        int maxGroupNum = GetMaxGroupNumber();
        if (maxGroupNum == 0) {
            // all players are admins
            Debug.Log("all players are admins; not creating any panels");
            return;
        }

        // for each group, keep track of min max of each student position to position the table
        List<MinMax> groupBounds = new List<MinMax>(maxGroupNum+1); // pass in capacity
        // this list needs to be filled
        for (int i = 0; i < maxGroupNum+1; i++) {
            groupBounds.Add(new MinMax());
            // groupBounds[i].InitializePointsArray();
        }

        // get PlayerHead game objects instead of the players list so we know where they are too
        GameObject[] playerHeadObjects = GameObject.FindGameObjectsWithTag("PlayerHead");
        Debug.Log(playerHeadObjects.Length);

        // var players = PhotonNetwork.CurrentRoom.Players.Values;
        // foreach (Player player in players) {
        foreach (GameObject playerHeadObject in playerHeadObjects) {
            Debug.Log(playerHeadObject);
            Player player = GetPlayerFromPlayerHeadObject(playerHeadObject);
            int groupNumber = GetPlayerGroupNumber(player);

            // if the player is the current player, then skip
            if (player.Equals(PhotonNetwork.LocalPlayer)) {
                Debug.Log("(skipping current player)");
                continue;
            }
            // skip players of group number 0 (admins)
            if (groupNumber == 0) {
                Debug.Log("skipping player with group number 0: " + player.NickName);
                continue;
            }

            // where are the players? they are at the position of the playerHeadObject
            // after filtering, build bounds list depending on group number
            groupBounds[groupNumber].AddPoint(playerHeadObject.transform.position);
        }

        // now that we have all of the group bounding boxes
        for (int i = 1; i < groupBounds.Count; i++) {
            // Debug.Log(groupBounds[i].points);
            // foreach (Vector3 point in groupBounds[i].points) {
            //     Debug.Log(point);
            // }
            if (groupBounds[i].points.Count == 0) {
                Debug.Log("there are no students in group " + i + ", skipping creation of panel");
                continue;
            }
            
            Vector3 max = groupBounds[i].max;
            Vector3 min = groupBounds[i].min;
            Vector3 center = (min + max) / 2;
            // Bounds b = new Bounds(center, max-min); // (Vector3 center, Vector3 size)

            // place table to the right of the group
            // max.x is the right of the bounding box
            Vector3 panelPos = new Vector3(max.x + 1, center.y+0.5f, center.z);
            // Vector3 panelPos = groupBounds[i].points[0] + new Vector3(1, 0, 0); // test code
            
            GameObject panelObject = PhotonNetwork.Instantiate(panelPrefab.name, panelPos, Quaternion.identity);
            // rotate it to face the group
            // TODO
            // it seems to face the right way right now so no need?


            // give the panel a group number!
            SetPhotonObjectGroupNumber(panelObject, i);
        }

    }
    
    public void DestroyAllPanels() {
        Debug.Log("DestroyAllPanels was called");

        GameObject[] panels = GameObject.FindGameObjectsWithTag("SidePanel");
        foreach (GameObject panel in panels) {
            // must claim ownership of panel first to destroy it (otherwise it fails if a student owns the photon view)

            PhotonView photonView = panel.GetComponent<PhotonView>();
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer); // transfer to instructor

            PhotonNetwork.Destroy(panel);
        }

    }


    // 0 means all groups
    // -1 means no group selected
    public void SetInstructorPanelCurrentGroup(int groupNumber) {
        SetRoomCustomProperty("InstructorPanelCurrentGroup", groupNumber);
    }


    // returns a list of integers: all the groups requesting for help
    public List<int> GetGroupsRequestingHelp() {

        List<int> groupsRequestingHelp = new();

        // also add -1 to give an option to give help to no group (-1 means no group selected)
        groupsRequestingHelp.Add(-1);
        // also add 0 to give an option to give help to all groups
        groupsRequestingHelp.Add(0);

        for (int i = 1; i <= GetMaxGroupNumber(); i++) {

            string key = "RequestHelpGroup" + i;
            // Debug.Log($"has key: {key}; {RoomHasCustomProperty(key)}");
            if (RoomHasCustomProperty(key) && (bool)GetRoomCustomProperty(key)) {
                // Debug.Log($"key value: {key}; {(bool)GetRoomCustomProperty(key)}");
                groupsRequestingHelp.Add(i);
            }

        }

        return groupsRequestingHelp;
    }

    // marks this group as no longer needing help
    public void HelpingCompleted(int groupNumber) {
        string key = "RequestHelpGroup" + groupNumber;
        SetRoomCustomProperty(key, false);
    }


    public void DestroyTablesAndMarkers() {

        // copied from additional functions


        // get all desks
        GameObject[] alignedTableObjects = GameObject.FindGameObjectsWithTag("AlignedTable");

        // delete all desks and spatial anchors
        foreach (GameObject alignedTable in alignedTableObjects) {
            alignedTable.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer); // first transfer ownership so that a second admin can destroy tables
            alignedTable.GetComponent<AlignedTable>().DestroyThisAndAnchor();
        }

        // delete all markers
        GameObject[] seatMarkerObjects = GameObject.FindGameObjectsWithTag("SeatMarker");
        foreach (GameObject seatMarker in seatMarkerObjects) {
            seatMarker.GetPhotonView().TransferOwnership(PhotonNetwork.LocalPlayer); // first transfer ownership so that a second admin can destroy markers
            // Destroy(seatMarkers);
            PhotonNetwork.Destroy(seatMarker); // cloud object; call cloud destroy
        }
    }
}
