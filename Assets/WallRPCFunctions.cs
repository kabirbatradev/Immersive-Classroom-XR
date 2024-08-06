using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WallRPCFunctions : MonoBehaviour
{
    [PunRPC]
    public void ActivateThickness(float x_scale)
    {
        GameObject thickness = transform.GetChild(0).gameObject;
        thickness.SetActive(true);
        thickness.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x_scale, 1);
    }
}
