
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
    private Transform spawnPoint;

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
}