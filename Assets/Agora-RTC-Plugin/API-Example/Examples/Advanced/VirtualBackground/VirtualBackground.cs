using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Agora.Rtc;
using System.IO;
using io.agora.rtc.demo;
using UnityEngine.Networking;

// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Serialization;
// using Agora.Rtc;
// using System.IO;

namespace Agora_RTC_Plugin.API_Example
{
    public class VirtualBackground : MonoBehaviour
    {
        [FormerlySerializedAs("appIdInput")]
        [SerializeField]
        private AppIdInput _appIdInput;

        [Header("_____________Basic Configuration_____________")]
        [FormerlySerializedAs("APP_ID")]
        [SerializeField]
        private string _appID = "";

        [FormerlySerializedAs("TOKEN")]
        [SerializeField]
        private string _token = "";

        [FormerlySerializedAs("CHANNEL_NAME")]
        [SerializeField]
        private string _channelName = "";

        internal IRtcEngine RtcEngine = null;

        private static int groupCount = 3;

        // Use this for initialization
        private void Awake()
        {
            LoadAssetData();
            InitEngine();
            JoinChannel();

            PermissionHelper.RequestMicrophontPermission();
            PermissionHelper.RequestCameraPermission();
        }

        // Update is called once per frame
        private void Update()
        {
            // PermissionHelper.RequestMicrophontPermission();
            // PermissionHelper.RequestCameraPermission();
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
            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new UserEventHandler(this);
            RtcEngineContext context = new RtcEngineContext(_appID, 0,
                                        CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                        AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            RtcEngine.Initialize(context);
            RtcEngine.InitEventHandler(handler);
        }

        private void JoinChannel()
        {
            RtcEngine.EnableAudio();
            RtcEngine.EnableVideo();
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            RtcEngine.JoinChannel(_token, _channelName);
        }

        private void OnDestroy()
        {
            if (RtcEngine == null) return;
            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
        }

        internal string GetChannelName()
        {
            return _channelName;
        }

        #region -- Video Render UI Logic ---

        internal static void MakeVideoView(uint uid, string channelId = "")
        {
            for (int i = 0; i < groupCount; i++) {
                var go = GameObject.Find(uid.ToString() + i);
                if (!ReferenceEquals(go, null))
                {
                    return; // reuse
                }

                // create a GameObject and assign to this new user
                var videoSurface = MakePlaneSurface(uid.ToString(), i);
                if (ReferenceEquals(videoSurface, null)) return;

                // configure videoSurface
                videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);

                videoSurface.SetEnable(true);
            }
        }

        private static VideoSurface MakePlaneSurface(string goName, int index)
        {
            // var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            var go = GameObject.Find("TestPlane" + index);

            if (go == null)
            {
                return null;
            }

            go.name = goName + index;
            var mesh = go.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                mesh.material = new Material(Shader.Find("Unlit/Texture"));
            }
            // set up transform
            go.transform.Rotate(0.0f, 0.0f, 0.0f);
    
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

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly VirtualBackground _sample;

        internal UserEventHandler(VirtualBackground sample)
        {
            _sample = sample;
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            VirtualBackground.DestroyVideoView(0);
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.LogError("Joined channel rahhh rahhh rahhh");
            Debug.LogError("uid" + uid);
            VirtualBackground.MakeVideoView(uid, _sample.GetChannelName());
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            VirtualBackground.DestroyVideoView(uid);
        }
    }

    #endregion
}