using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Agora.Rtc;
using Photon.Pun;

public class AgoraPanelScript : MonoBehaviour
{

    public const string instructorPanelCurrentGroupKey = "InstructorPanelCurrentGroup";
    private bool currentInAgora = false;
    private MeshRenderer renderer;
    private VideoSurface videoSurface;

    private int thisPanelGroupNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        if (AgoraManager.Instance == null) {
            SampleController.Instance.Log("ERROR: AgoraManager.Instance is null");
            gameObject.SetActive(false);
            return;
        }
        
        thisPanelGroupNumber = GetThisPanelGroupNumber();
        SampleController.Instance.Log("Initializing an Agora Panel of group number " + thisPanelGroupNumber);
        
        videoSurface = gameObject.AddComponent<VideoSurface>(); // important
        renderer = GetComponent<MeshRenderer>();
        renderer.enabled = false;
        
        StartCoroutine(CheckRoomProperties());
    }

    private IEnumerator CheckRoomProperties()
    {
        while (true)
        {
            // check room custom property: InstructorPanelCurrentGroup
            
            bool shouldBeInAgora = false;

            if (CloudFunctions.RoomHasCustomProperty(instructorPanelCurrentGroupKey))
            {
                int instructorGroupNumber = (int)CloudFunctions.GetRoomCustomProperty(instructorPanelCurrentGroupKey);
                
                // SampleController.Instance.Log("Checking Room Property for " + thisPanelGroupNumber);
                if (instructorGroupNumber == thisPanelGroupNumber) // instructorGroupNumber == 0 || 
                {
                    shouldBeInAgora = true;
                }
            }
            if (!currentInAgora && shouldBeInAgora)
            {
                if (!AgoraManager.Instance.isInAgoraRoom)
                {
                    AgoraManager.Instance.JoinChannel();
                }
                else
                {
                    JoinChannelEventTriggered();
                }
            }
            else if (currentInAgora && !shouldBeInAgora)
            {
                if (AgoraManager.Instance.isInAgoraRoom && CloudFunctions.GetCurrentGroupNumber() != 0)
                {
                    AgoraManager.Instance.LeaveChannel();
                }
                LeaveChannel();
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void JoinChannelEventTriggered()
    {
        if (AgoraManager.Instance.globalUID == 0) {
            // Agora is not ready yet (not connected)
            SampleController.Instance.Log("ERROR: agora is not connected yet because global uid is 0");
            gameObject.SetActive(false);
            return;
        }
        SampleController.Instance.Log("Join Channel event triggered for panel " + thisPanelGroupNumber);
        uint uid = (uint)AgoraManager.Instance.globalUID;
        string channelId = AgoraManager.Instance.GetChannelName();
        videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        videoSurface.SetEnable(true);
        renderer.enabled = true;
        currentInAgora = true;
    }
    
    private void LeaveChannel()
    {
        videoSurface.SetEnable(false);
        renderer.enabled = false;
        currentInAgora = false;
    }
    
    private int GetThisPanelGroupNumber() {
        // get parent panel object
        if (gameObject.transform.parent == null) {
            Debug.Log("agora panel has no parent");
            return 1;
        }
        GameObject parentPanel = gameObject.transform.parent.gameObject;
        // // get the photon view
        // PhotonView parentPhotonView = parentPanel.GetPhotonView();
        // get photon object group number
        if (CloudFunctions.PhotonObjectHasGroupNumber(parentPanel)) {
            int panelGroupNumber = CloudFunctions.GetPhotonObjectGroupNumber(parentPanel);
            return panelGroupNumber;
        }
        // otherwise, assume it is 1
        Debug.Log("panel does not have a group number");
        return 1;
    }

}
