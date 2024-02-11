using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStudentCam : MonoBehaviour
{
    public string filePath;

    public void startStudentCam()
    {
        System.Diagnostics.Process.Start(filePath);
    }
}
