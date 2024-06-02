using UnityEngine;

public class SetDeviceVolume : MonoBehaviour
{
    public float targetVolume = 0.3f;
    void Start()
    {
        var a = new AndroidNativeVolumeService();
        a.SetSystemVolume(targetVolume);
    }
    
}
