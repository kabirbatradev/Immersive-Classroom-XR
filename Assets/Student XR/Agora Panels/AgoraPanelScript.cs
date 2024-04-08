using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Agora.Rtc;

public class AgoraPanelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (AgoraManager.Instance == null || AgoraManager.Instance.globalUID == 0) {
            // Agora is not ready yet (not connected)
            SampleController.Instance.Log("ERROR: failed to initialize agora panel (agora might not be connected yet)");
            gameObject.SetActive(false);
        }
        uint uid = AgoraManager.Instance.globalUID;
        string channelId = AgoraManager.Instance.GetChannelName();
        var videoSurface = gameObject.AddComponent<VideoSurface>(); // important
        videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        videoSurface.SetEnable(true);
    }

}
