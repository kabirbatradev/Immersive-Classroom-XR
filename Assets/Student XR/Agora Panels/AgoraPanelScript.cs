using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Agora.Rtc;
using Photon.Pun;

public class AgoraPanelScript : MonoBehaviour
{

    public const string instructorPanelCurrentGroupKey = "InstructorPanelCurrentGroup";
    private bool panelIsVisible;
    private MeshRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        if (AgoraManager.Instance == null) {
            SampleController.Instance.Log("ERROR: AgoraManager.Instance is null");
            gameObject.SetActive(false);
            return;
        }
        if (AgoraManager.Instance.globalUID == 0) {
            // Agora is not ready yet (not connected)
            SampleController.Instance.Log("ERROR: agora is not connected yet because global uid is 0");
            gameObject.SetActive(false);
            return;
        }

        int thisPanelGroupNumber = GetThisPanelGroupNumber();
        SampleController.Instance.Log("Initializing an Agora Panel of group number " + thisPanelGroupNumber);
        
        uint uid = (uint)AgoraManager.Instance.globalUID;
        string channelId = AgoraManager.Instance.GetChannelName();
        var videoSurface = gameObject.AddComponent<VideoSurface>(); // important
        videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        videoSurface.SetEnable(true);

        renderer = GetComponent<MeshRenderer>();
    }

    void Update() {
        // check room custom property: InstructorPanelCurrentGroup

        panelIsVisible = true;

        if (CloudFunctions.RoomHasCustomProperty(instructorPanelCurrentGroupKey)) {
            int instructorGroupNumber = (int)CloudFunctions.GetRoomCustomProperty(instructorPanelCurrentGroupKey);

            int thisPanelGroupNumber = GetThisPanelGroupNumber();

            if (instructorGroupNumber != 0 && instructorGroupNumber != thisPanelGroupNumber) {
                panelIsVisible = false;
            }
        }
        else {
            // the instructor hasn't even joined, so dont show the panel..? but then its not even possible to create a panel, so just show it for debugging purposes
            // panelIsVisible = false;
        }

        // to show or not to show the panel, that is the question.
        // do not disable the object itself: this script will stop updating
        // instead, disable the renderer
        renderer.enabled = panelIsVisible;

        // if the panel is visible, then also hear professor audio
        uint professorUid = (uint)AgoraManager.Instance.globalUID;
        AgoraManager.Instance.RtcEngine.MuteRemoteAudioStream(professorUid, !panelIsVisible);
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
