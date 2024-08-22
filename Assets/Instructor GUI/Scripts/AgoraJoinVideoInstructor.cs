using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Agora.Rtc;
using io.agora.rtc.demo;
using UnityEngine.Serialization;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
public class AgoraJoinVideoInstructor : MonoBehaviour
{
    private ArrayList permissionList = new ArrayList() { Permission.Camera, Permission.Microphone };

    [SerializeField]
    private AppIdInput _appIdInput;

    [SerializeField]
    private string _webcamName = "OBS Virtual Camera"; //"OBS Virtual Camera";

    [SerializeField]
    private Vector2Int _resolution = new Vector2Int(600, 600); // Resolution of Video

    private string _appID = "";
    private string _token = "";
    private string _channelName = "";


    internal VideoSurface ThisView;
    internal IRtcEngine RtcEngine;

    void Awake()
    {
        //CheckPermissions();
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadAssetData();
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
        ListAvailableWebcams();
        SetWebCamByName(_webcamName);
    }

    [ContextMenu("ShowAgoraBasicProfileData")]
    private void LoadAssetData()
    {
        if (_appIdInput == null) return;
        _appID = _appIdInput.appID;
        _token = _appIdInput.token;
        _channelName = _appIdInput.channelName;

    }


    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }

    private void SetupVideoSDKEngine()
    {
        // Create an IRtcEngine instance
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new RtcEngineContext();
        context.appId = _appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        // Initialize the instance
        RtcEngine.Initialize(context);
        // RtcEngine.DisableAudio();
    }

    // Create a user event handler instance and set it as the engine event handler
    private void InitEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }

    private void SetupUI()
    {
        GameObject go = GameObject.Find("ThisView");
        ThisView = go.AddComponent<VideoSurface>();
        go.transform.Rotate(0.0f, 0.0f, -180.0f);
        go = GameObject.Find("LeaveVideoBtn");
        go.GetComponent<Button>().onClick.AddListener(Leave);
        go = GameObject.Find("JoinVideoBtn");
        go.GetComponent<Button>().onClick.AddListener(Join);
    }

    void SetWebCamByName(string webcamName)
    {
        // Get the list of available video devices
        var videoDeviceManager = RtcEngine.GetVideoDeviceManager();
        var deviceList = videoDeviceManager.EnumerateVideoDevices();

        foreach (var device in deviceList)
        {
            if (device.deviceName == webcamName)
            {
                videoDeviceManager.SetDevice(device.deviceId);
                Debug.Log("Selected Device: " + device.deviceName);
                break;
            }
        }
    }

    void ListAvailableWebcams()
    {
        var videoDeviceManager = RtcEngine.GetVideoDeviceManager();
        var deviceList = videoDeviceManager.EnumerateVideoDevices();

        foreach (var device in deviceList)
        {
            Debug.Log("Device Name: " + device.deviceName);
        }
    }

    public void Join()
    {
        // Enable the video module
        RtcEngine.EnableVideo();
        // Set channel media options
        ChannelMediaOptions options = new ChannelMediaOptions();
        // Start video rendering
        ThisView.SetEnable(true);
        // Automatically subscribe to all audio streams
        options.autoSubscribeAudio.SetValue(true);
        // Automatically subscribe to all video streams
        options.autoSubscribeVideo.SetValue(true);
        // Set the channel profile to live broadcast
        options.channelProfile.SetValue(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        //Set the user role as host
        options.clientRoleType.SetValue(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        // Set video encoder configuration
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(_resolution.x, _resolution.y);
        RtcEngine.SetVideoEncoderConfiguration(config);
        // Join a channel
        RtcEngine.JoinChannel(_token, _channelName, 0, options);
    }

    public void Leave()
    {
        Debug.Log("Leaving " + _channelName);
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Disable the video module
        RtcEngine.DisableVideo();
        // Stop local video rendering
        ThisView.SetEnable(false);
    }

    void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            // Destroy IRtcEngine
            RtcEngine.Dispose();
            RtcEngine = null;
        }
    }

    // Implement your own EventHandler class by inheriting the IRtcEngineEventHandler interface class implementation
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraJoinVideoInstructor _videoSample;
        internal UserEventHandler(AgoraJoinVideoInstructor videoSample)
        {
            _videoSample = videoSample;
        }

        // error callback
        public override void OnError(int err, string msg)
        {
            Debug.LogError(msg);
        }

        // Triggered when a local user successfully joins the channel
        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            _videoSample.ThisView.SetForUser(0, "");
        }

        // When the SDK receives the first frame of a remote video stream and successfully decodes it, the OnUserJoined callback is triggered.
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log("Remote user joined");
        }

        // This callback is triggered when a remote user leaves the current channel
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log("Remote user offline");
        }


    }
}
