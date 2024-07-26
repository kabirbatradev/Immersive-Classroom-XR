using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PanelRPCFunctions : MonoBehaviour
{
    [SerializeField] private InteractivePanelLogic script;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void UpdatePanelContent(string[] options)
    {
        // update the rendering for everyone
        script.RenderToggleValueChanged(options[2]);
        if (photonView.IsMine)
        {
            // the instructor gui in editor
            Debug.Log("User " + options[0] + " from group " + options[1] + " choose option " + options[2]);
            // also record this in a local json file
        }
    }

    public void OnUserChangeOptions(string option)
    {
        if (!photonView.IsMine && CloudFunctions.GetCurrentGroupNumber() != 0)
        {
            // not the instructor gui, which is the owner
            // group number != 0, not the cameraman
            // If you can still chang ethe panel, which means that you are in the student group
            string[] options = {PhotonNetwork.LocalPlayer.NickName,
                CloudFunctions.GetCurrentGroupNumber().ToString(), option};
            photonView.RPC("UpdatePanelContent", RpcTarget.All, options);
        }
    }
    
    public void OnRequestInstructorForHelpPressed() {
        SampleController.Instance.Log("OnRequestInstructorForHelpPressed");

        int groupNumber = CloudFunctions.GetCurrentGroupNumber();
        string key = "RequestHelpGroup" + groupNumber;

        bool updatedValue = true;
        // if the custom property already exists, then just toggle it
        if (CloudFunctions.RoomHasCustomProperty(key)) {
            updatedValue = !(bool)CloudFunctions.GetRoomCustomProperty(key);
        }

        SampleController.Instance.Log("setting request instructor help to " + updatedValue);
        CloudFunctions.SetRoomCustomProperty(key, updatedValue);

    }
}
