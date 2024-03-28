using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExternal : MonoBehaviour
{
    public string filePath;
    public void StartExternalApp()
    {
        System.Diagnostics.Process.Start(filePath);
        Debug.Log(System.IO.Directory.GetCurrentDirectory());
    }
}
