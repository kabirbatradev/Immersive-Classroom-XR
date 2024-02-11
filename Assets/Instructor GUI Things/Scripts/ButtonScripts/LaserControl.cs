using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    public Laser laserScript;

    public void CameraLaserMode()
    {
        laserScript.laserFromCamera = true;
    }
    public void AnchorLaserMode()
    {
        laserScript.laserFromCamera = false;
    }
}
