using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AlignedTable : MonoBehaviour
{

    private PhotonView photonView;

    [SerializeField]
    private OVRSpatialAnchor anchorPrefab;

    private SharedAnchor tableAnchor;

    private List<MeshRenderer> allMeshRenderers;

    // Start is called before the first frame update
    void Start()
    {
        // if the table is a local table (not instantiated by another device)
        // then create a spatial anchor
        photonView = gameObject.GetPhotonView();

        if (photonView.IsMine) {
            // this table was created by this client

            // create a new anchor at the position of this table
            tableAnchor = Instantiate(anchorPrefab, transform.position, transform.rotation).GetComponent<SharedAnchor>();



            // allMeshRenderers
            foreach (Transform visual in tableAnchor.transform) {
                allMeshRenderers.Add(visual.gameObject.GetComponent<MeshRenderer>());
            }
        }

        allMeshRenderers.Add(GetComponent<MeshRenderer>());

        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine) {
            // this table was created by this client
            // update the position of the table so that if a recenter/realignment occurs, the table is still attached to the anchor

            transform.SetPositionAndRotation(tableAnchor.transform.position, tableAnchor.transform.rotation);

            // transform.position = tableAnchor.transform.position;
            // transform.rotation = tableAnchor.transform.rotation;
        }
    }

    
    public void DestroyThisAndAnchor() {
        if (tableAnchor != null) Destroy(tableAnchor.gameObject); // local object; call local destroy
        // Destroy(gameObject);
        PhotonNetwork.Destroy(gameObject); // cloud object; call cloud destroy
    }

    
    // when hiding and showing anchor, do not enable and disable the objects; the table cloud object will not be positioned correctly
    public void HideThisAndAnchor() {

        foreach (MeshRenderer renderer in allMeshRenderers) {
            renderer.enabled = false;
        }

        // if (photonView.IsMine) {
        //     // visual axis and panel are both children of the anchor object
        //     foreach (Transform visual in tableAnchor.transform) {
        //         visual.gameObject.SetActive(false);
        //     }
        // }

        // gameObject.SetActive(false);

    }
    public void ShowThisAndAnchor() {

        foreach (MeshRenderer renderer in allMeshRenderers) {
            renderer.enabled = true;
        }

        // if (photonView.IsMine) {
        //     // visual axis and panel are both children of the anchor object
        //     foreach (Transform visual in tableAnchor.transform) {
        //         visual.gameObject.SetActive(true);
        //     }
        // }

        // gameObject.SetActive(true);
    }
}
