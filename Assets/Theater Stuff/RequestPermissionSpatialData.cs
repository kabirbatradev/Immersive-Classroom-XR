using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestPermissionSpatialData : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }




    void Denied(string permission)  => Debug.Log($"{permission} Denied");
    void Granted(string permission) => Debug.Log($"{permission} Granted");

    void Start()
    {
        const string spatialPermission = "com.oculus.permission.USE_SCENE";
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(spatialPermission))
        {
            var callbacks = new UnityEngine.Android.PermissionCallbacks();
            callbacks.PermissionDenied += Denied;
            callbacks.PermissionGranted += Granted;

            // avoid callbacks.PermissionDeniedAndDontAskAgain. PermissionDenied is
            // called instead unless you subscribe to PermissionDeniedAndDontAskAgain.

            UnityEngine.Android.Permission.RequestUserPermission(spatialPermission, callbacks);
        }
    }
}
