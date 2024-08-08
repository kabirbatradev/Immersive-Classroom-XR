
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Agora.Rtc;
using System.IO;
using io.agora.rtc.demo;
using UnityEngine.Networking;
using System;

// to make the permission helper work
using Agora_RTC_Plugin.API_Example;
using ExitGames.Client.Photon.StructWrapping;

public class AgoraManager : MonoBehaviour
{
    [FormerlySerializedAs("appIdInput")]
    [SerializeField]
    private AppIdInput _appIdInput;

    // [Header("_____________Basic Configuration_____________")]
    // [FormerlySerializedAs("APP_ID")]
    // [SerializeField]
    private string _appID = "";

    // [FormerlySerializedAs("TOKEN")]
    // [SerializeField]
    private string _token = "";

    // [FormerlySerializedAs("CHANNEL_NAME")]
    // [SerializeField]
    private string _channelName = "";

    // public Text LogText;
    // internal Logger Log;

    
    // make the RtcEngine accessable from other scripts
    [NonSerialized]
    public IRtcEngine RtcEngine = null;

    [NonSerialized]
    public int globalUID = 0;

    public bool isInAgoraRoom = false;
    public GameObject agoraPanelPrefab;



    // make this class a singleton:
    public static AgoraManager Instance;
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }


    // Use this for initialization
    private void Start()
    {
        SampleController.Instance.Log("starting Agora Manager after photon connects");
        LoadAssetData();
        InitEngine();
        // JoinChannel();

        // PermissionHelper.RequestMicrophontPermission();
        // PermissionHelper.RequestCameraPermission();
//             LoadAssetData();
//             if (CheckAppId())
//             {
//                 InitEngine();
//                 InitLogFilePath();
//                 // SetupUI();
//                 JoinChannel();

//                 string fromFile = Path.Combine(Application.streamingAssetsPath, "img/png.png");
//                 string toFile = Path.Combine(Application.persistentDataPath, "png.png");

// #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//                 fromFile = fromFile.Replace('/', '\\');
//                 toFile = toFile.Replace('/', '\\');
// #endif
//                 this.CopyFile(fromFile, toFile);
//                 Log.UpdateLog("File copy finish: " + toFile);
//             }
    }

    //Show data in AgoraBasicProfile
    [ContextMenu("ShowAgoraBasicProfileData")]
    private void LoadAssetData()
    {
        if (_appIdInput == null) return;
        _appID = _appIdInput.appID;
        _token = _appIdInput.token;
        _channelName = _appIdInput.channelName;

    }
    
    private void InitEngine()
    {
        SampleController.Instance.Log("Init Engine Agora Manager.");
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventCallbackHandler handler = new UserEventCallbackHandler(this);
        RtcEngineContext context = new RtcEngineContext();
        context.appId = _appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        context.areaCode = AREA_CODE.AREA_CODE_GLOB;
        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    public void JoinChannel()
    {
        SampleController.Instance.Log("Join Channel Agora Manager.");
        // RtcEngine.EnableAudio();
        RtcEngine.EnableVideo(); 

        RtcEngine.MuteLocalAudioStream(true); // mute the audio of the student

        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
        RtcEngine.JoinChannel(_token, _channelName, "", 0);
    }
    
    public void LeaveChannel()
    {
        SampleController.Instance.Log("Leave Channel Agora Manager.");
        Debug.Log("Leaving " + _channelName);
        // Leave the channel
        RtcEngine.LeaveChannel();
        // Disable the video module
        RtcEngine.DisableVideo();
    }

//         private void InitLogFilePath()
//         {
//             var path = Application.persistentDataPath + "/rtc.log";
// #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//              path = path.Replace('/', '\\');
// #endif
//             var nRet = RtcEngine.SetLogFile(path);
//             this.Log.UpdateLog(string.Format("logPath:{0},nRet:{1}", path, nRet));
//         }

    // private void SetupUI()
    // {
    //     var ui = this.transform.Find("UI");

    //     var btn = ui.Find("StartButton").GetComponent<Button>();
    //     btn.onClick.AddListener(OnStartButtonPress);

    //     btn = ui.Find("StartButton2").GetComponent<Button>();
    //     btn.onClick.AddListener(OnStartButtonPress2);

    //     btn = ui.Find("StopButton").GetComponent<Button>();
    //     btn.onClick.AddListener(OnStopButtonPress);
    // }


    // private void OnStartButtonPress()
    // {
    //     var source = new VirtualBackgroundSource();
    //     source.background_source_type = BACKGROUND_SOURCE_TYPE.BACKGROUND_COLOR;
    //     source.color = 0xffffff;
    //     var segproperty = new SegmentationProperty();
    //     var nRet = RtcEngine.EnableVirtualBackground(true, source, segproperty, MEDIA_SOURCE_TYPE.PRIMARY_CAMERA_SOURCE);
    //     this.Log.UpdateLog("EnableVirtualBackground true :" + nRet);
    // }

//         private void OnStartButtonPress2()
//         {
//             var source = new VirtualBackgroundSource();

//             source.background_source_type = BACKGROUND_SOURCE_TYPE.BACKGROUND_IMG;
//             string filePath = Path.Combine(Application.persistentDataPath, "png.png");
// #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//             filePath = filePath.Replace('/', '\\');   
// #endif
//             source.source = filePath;

//             var segproperty = new SegmentationProperty();
//             var nRet = RtcEngine.EnableVirtualBackground(true, source, segproperty, MEDIA_SOURCE_TYPE.PRIMARY_CAMERA_SOURCE);
//             this.Log.UpdateLog("EnableVirtualBackground true :" + nRet);
//         }

    // private void OnStopButtonPress()
    // {
    //     var source = new VirtualBackgroundSource();
    //     var segproperty = new SegmentationProperty();
    //     var nRet = RtcEngine.EnableVirtualBackground(false, source, segproperty, MEDIA_SOURCE_TYPE.PRIMARY_CAMERA_SOURCE);
    //     this.Log.UpdateLog("EnableVirtualBackground false :" + nRet);
    // }


    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        LeaveChannel();
        if (RtcEngine == null) return;
        RtcEngine.InitEventHandler(null);
        // RtcEngine.LeaveChannel();
        RtcEngine.Dispose();
    }

    internal string GetChannelName()
    {
        return _channelName;
    }
    

    #region -- Video Render UI Logic ---


    // my custom test function that creates a new place and puts the agora video on it
    public void TestCreateNewAgoraPanel(uint uid, string channelId = "") {
        // GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        GameObject plane = Instantiate(agoraPanelPrefab);
        // plane.transform.localScale = new Vector3(0.1f,0.1f,0.1f); // shrink the plane
        // plane.transform.Rotate(-90f, 0, 0); // rotate about x axis by 90 degrees
        
        // var mesh = plane.GetComponent<MeshRenderer>();
        // if (mesh != null) {
        //     mesh.material = new Material(Shader.Find("Unlit/Texture")); // important
        // }

        // configure videoSurface (should be automatic now)
        // var videoSurface = plane.AddComponent<VideoSurface>(); // important

        // videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);

        // videoSurface.SetEnable(true);
    }

    internal static void MakeVideoView(uint uid, string channelId = "")
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        var videoSurface = MakeImageSurface(uid.ToString());
        if (ReferenceEquals(videoSurface, null)) return;
        // configure videoSurface
        if (uid == 0)
        {
            videoSurface.SetForUser(uid, channelId);
        }
        else
        {
            videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }

        videoSurface.OnTextureSizeModify += (int width, int height) =>
        {
            float scale = (float)height / (float)width;
            videoSurface.transform.localScale = new Vector3(-5, 5 * scale, 1);
            Debug.Log("OnTextureSizeModify: " + width + "  " + height);
        };

        videoSurface.SetEnable(true);





        // for (int i = 0; i < groupCount; i++) {
        //     var go = GameObject.Find(uid.ToString() + i);
        //     if (!ReferenceEquals(go, null))
        //     {
        //         return; // reuse
        //     }

        //     // create a GameObject and assign to this new user
        //     var videoSurface = MakePlaneSurface(uid.ToString(), i);
        //     if (ReferenceEquals(videoSurface, null)) return;

        //     // configure videoSurface
        //     videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);

        //     videoSurface.SetEnable(true);
        // }
    }

    // VIDEO TYPE 1: 3D Object
    private static VideoSurface MakePlaneSurface(string goName)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        var mesh = go.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            Debug.LogWarning("VideoSureface update shader");
            mesh.material = new Material(Shader.Find("Unlit/Texture")); // important
        }
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        go.transform.position = Vector3.zero;
        go.transform.localScale = new Vector3(0.25f, 0.5f, 0.5f);

        // configure videoSurface
        var videoSurface = go.AddComponent<VideoSurface>(); // important
        return videoSurface;
    }

    // Video TYPE 2: RawImage
    private static VideoSurface MakeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;
        // to be renderered onto
        go.AddComponent<RawImage>();
        // make the object draggable
        go.AddComponent<UIElementDrag>();
        var canvas = GameObject.Find("VideoCanvas");
        if (canvas != null)
        {
            go.transform.parent = canvas.transform;
            Debug.Log("add video view");
        }
        else
        {
            Debug.Log("Canvas is null video view");
        }

        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(2f, 3f, 1f);

        // configure videoSurface
        var videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    internal static void DestroyVideoView(uint uid)
    {
        var go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            Destroy(go);
        }
    }

    #endregion
}

#region -- Agora Event ---

internal class UserEventCallbackHandler : IRtcEngineEventHandler
{
    private readonly AgoraManager _sample;

    internal UserEventCallbackHandler(AgoraManager sample)
    {
        _sample = sample;
    }

    // public override void OnError(int err, string msg)
    // {
    //     _sample.Log.UpdateLog(string.Format("OnError err: {0}, msg: {1}", err, msg));
    // }

    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        _sample.isInAgoraRoom = true;
        // int build = 0;
        // Debug.Log("Agora: OnJoinChannelSuccess ");
        // _sample.Log.UpdateLog(string.Format("sdk version: ${0}",
        //     _sample.RtcEngine.GetVersion(ref build)));
        // _sample.Log.UpdateLog(
        //     string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
        //                     connection.channelId, connection.localUid, elapsed));

        // VirtualBackground.MakeVideoView(0); // default uid for local feed 

    }

    // public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
    // {
    //     _sample.Log.UpdateLog("OnRejoinChannelSuccess");
    // }

    public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
    {
        _sample.isInAgoraRoom = false;
        // _sample.Log.UpdateLog("OnLeaveChannel");
        // VirtualBackground.DestroyVideoView(0);
    }

    // public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole, ClientRoleOptions newRoleOptions)
    // {
    //     _sample.Log.UpdateLog("OnClientRoleChanged");
    // }

    public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
    {

        SampleController.Instance.Log("on user joined");

        if (_sample.globalUID != 0) {
            // if its not 0, then its already set so dont change anything
            SampleController.Instance.Log("uid is not 0 so it is not 0 so will not change uid");
        }


        // _sample.Log.UpdateLog(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
        // VirtualBackground.MakeVideoView(uid, _sample.GetChannelName());


        // first check if there is already a custom room property with the agora uid
        // if not, then set it and this variable
        // if it does exist, then use that instead
        if (CloudFunctions.RoomHasCustomProperty("AgoraUID")) {
            int cloudUID = (int)CloudFunctions.GetRoomCustomProperty("AgoraUID");
            _sample.globalUID = (int)cloudUID;

            SampleController.Instance.Log("user joined; room has custom property AgoraUID, setting uid to cloud value: " + cloudUID);
        }
        else {
            // failed because the room doesnt exist yet?
            CloudFunctions.SetRoomCustomProperty("AgoraUID", (int)uid);
            _sample.globalUID = (int)uid;

            SampleController.Instance.Log("user joined; room does not have custom property AgoraUID, setting cloud value: " + uid);
        }

        // also mute the instructor using their uid (we can unmute it automatically in update function of panel
        // AgoraManager.Instance.RtcEngine.MuteRemoteAudioStream(uid, true);

        GameObject.FindWithTag("SidePanel").transform.GetChild(1).GetComponent<AgoraPanelScript>().JoinChannelEventTriggered();
        // _sample.TestCreateNewAgoraPanel(uid, _sample.GetChannelName());
    }

    public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
    {
        // _sample.Log.UpdateLog(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
        //     (int)reason));
        AgoraManager.DestroyVideoView(uid);
    }
}

#endregion
