
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

    public void Update() {
        // SampleController.Instance.Log("this is a test");

        // if (alignTableMode) {

            // bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
            bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.A);
            // OVRInput.RawButton
            
            if (buttonPressed) {
                SampleController.Instance.Log("A button presssed!");
            }
        // }

    }





    public void OnSpawnSphereButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnSphereButtonPressed");

        SpawnSphere();
    }

    private void SpawnSphere()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(spherePrefab.name, spawnPoint.position, spawnPoint.rotation);
        var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        photonGrabbable.TransferOwnershipToLocalPlayer();
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

        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();



        alignTableMode = true;
        SampleController.Instance.Log(alignTableMode.ToString());
        
    }
}