using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PanelRPCFunctions : MonoBehaviour
{
    [SerializeField] private InteractivePanelLogic script;
    private PhotonView photonView;
    private string path;
    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            //string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            path = Application.dataPath + "/TrackedData/EventData.csv";
        }
    }

    public void OnUserChangedOptions(string option)
    {
        if (!photonView.IsMine && CloudFunctions.GetCurrentGroupNumber() != 0)
        {
            // not the instructor gui, which is the owner
            // group number != 0, not the cameraman
            // If you can still change the panel, which means that you are in the student group
            string nickname = PhotonNetwork.LocalPlayer.NickName;
            int groupNum = CloudFunctions.GetCurrentGroupNumber();
            photonView.RPC("UpdatePanelContent", RpcTarget.All, nickname, groupNum, option);
        }
    }
    
    [PunRPC]
    public void UpdatePanelContent(string nickname, int groupNum, string option)
    {
        // update the rendering for everyone
        script.RenderToggleValueChanged(option);
        if (photonView.IsMine)
        {
            // This is the instructor gui in editor
            // save the event to a local csv file
            string content = $"User {nickname} from group {groupNum} choose option {option}.";
            // Get the current timestamp
            string timestamp = DateTime.UtcNow.ToString("o");
            // Create the new row
            string newRow = $"{timestamp},{content}";
            // Append the new row to the CSV file
            File.AppendAllText(path, newRow + Environment.NewLine);
        }
    }
    
    public void OnUserRaisedHand(bool isOn)
    {
        if (!photonView.IsMine && CloudFunctions.GetCurrentGroupNumber() != 0)
        {
            // not the instructor gui, which is the owner
            // group number != 0, not the cameraman
            // If you can still change the panel, which means that you are in the student group
            string nickname = PhotonNetwork.LocalPlayer.NickName;
            int groupNum = CloudFunctions.GetCurrentGroupNumber();
            photonView.RPC("UpdateRaiseHandStatus", RpcTarget.All, nickname, groupNum, isOn);
        }
    }

    [PunRPC]
    public void UpdateRaiseHandStatus(string nickname, int groupNum,  bool isOn)
    {
        script.RenderRaiseHandStatusChanged(isOn);
        if (photonView.IsMine)
        {
            // This is the instructor gui in editor
            string key = "RequestHelpGroup" + groupNum;
            CloudFunctions.SetRoomCustomProperty(key, isOn);
            // save the event to a local csv file
            string content = $"User {nickname} from group {groupNum} toggle raise hand to {isOn}.";
            // Get the current timestamp
            string timestamp = DateTime.UtcNow.ToString("o");
            // Create the new row
            string newRow = $"{timestamp},{content}";
            // Append the new row to the CSV file
            File.AppendAllText(path, newRow + Environment.NewLine);
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
