using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
public class UserHeadSphereController : MonoBehaviour
{


    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = transform.parent.transform.position + new Vector3(0, 0.6f, 0);
        gameObject.transform.rotation = Quaternion.identity;

        // renderer.material.color = Color.red;


        GameObject headTracker = transform.parent.gameObject;
        var ownerPlayer = headTracker.GetComponent<PhotonPun.PhotonView>().Owner;

        int groupNumber = 0;
        if (ownerPlayer.CustomProperties.ContainsKey("groupNumber")) {
            groupNumber = (int)ownerPlayer.CustomProperties["groupNumber"];

            // Debug.Log("group number: " + groupNumber);
        }
        else {
            // Debug.Log("group number not set");
        }

        renderer.material.color = Color.HSVToRGB(1.0f * groupNumber / 10.0f, 1.0f, 1.0f);
        // renderer.material.color = Color.HSVToRGB(0.5f, 1.0f, 1.0f); // this does work



    }
}
