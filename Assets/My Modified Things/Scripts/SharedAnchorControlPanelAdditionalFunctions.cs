
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class SharedAnchorControlPanelAdditionalFunctions : MonoBehaviour
{

    [SerializeField]
    private GameObject spherePrefab;


    [SerializeField]
    private GameObject jengaPrefab;

    [SerializeField]
    private GameObject tablePrefab;


    [SerializeField]
    private Transform spawnPoint;




    private bool alignTableMode = false;
    private int countAButton = 0;

    private GameObject mostRecentSphere;

    public void Update() {

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



        // every frame, scan through all objects that have "object data" component
        // and enable or disable them based on if their group number matches the current 
        // user's group number
        // int currentUserGroupNumber = 0;
        int currentUserGroupNumber = gameObject.GetComponent<StudentData>().groupNumber;
        // need to include inactive (params are type, includeInactive)
        ObjectData[] allNetworkObjectDatas = (ObjectData[]) FindObjectsOfType(typeof(ObjectData), true);

        foreach (ObjectData objectData in allNetworkObjectDatas) {
            GameObject gameObject = objectData.gameObject;

            // if the group numbers match, then set the object to active
            if (objectData.groupNumber == currentUserGroupNumber) {
                gameObject.SetActive(true);
            }
            // if they do not match, disable the object
            else {
                gameObject.SetActive(false);
            }
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


        // print the sphere's group number if it has an "object data" property
        ObjectData data = sphereObject.GetComponent<ObjectData>();
        
        SampleController.Instance.Log("group number of this sphere is: " + data.groupNumber);



        mostRecentSphere = sphereObject;
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



        alignTableMode = true;
        // SampleController.Instance.Log(alignTableMode.ToString());
        
    }
}