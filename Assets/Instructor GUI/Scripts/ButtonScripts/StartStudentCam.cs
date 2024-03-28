using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StartStudentCam : MonoBehaviour
{
    public string filePath;

    public void startStudentCam()
    {
        //System.Diagnostics.Process.Start(filePath);
        Process process = new Process();

        process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

        string filename = System.IO.Directory.GetCurrentDirectory();
        print(filename);

        process.StartInfo.FileName = filename + "\\Stabilization\\windowCap.exe";
        
        try
        {
            process.Start();
            process.WaitForExit();
        }
        catch (Exception ex)
        {
            print(ex);
        }
    }
}
