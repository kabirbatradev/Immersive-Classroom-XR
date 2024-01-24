using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RotColor : MonoBehaviour
{
    public OVRCameraRig camRig;
    public Canvas canvas;
    //public Image RGBA;
    public TextMeshProUGUI debug;
    public Image xr;
    public Image xg;
    public Image xb;
    public Image yr;
    public Image yg;
    public Image yb;
    public Image zr;
    public Image zg;
    public Image zb;
    public Image wr;
    public Image wg;
    public Image wb;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = camRig.centerEyeAnchor.GetComponent<Camera>();
        canvas.worldCamera = cam;
        debug = canvas.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = cam.transform.rotation;

        // R.r = rot.x
        Vector3 x = RotationToColor(rot.x);
        Vector3 y = RotationToColor(rot.y);
        Vector3 z = RotationToColor(rot.z);
        Vector3 w = RotationToColor(rot.w);

        xr.color = new Color(x.x, 0, 0, 1);
        xg.color = new Color(0, x.y, 0, 1);
        xb.color = new Color(0, 0, x.z, 1);

        yr.color = new Color(y.x, 0, 0 , 1);
        yg.color = new Color(0, y.y, 0, 1);
        yb.color = new Color(0, 0, y.z, 1);

        zr.color = new Color(z.x, 0, 0, 1);
        zg.color = new Color(0, z.y, 0, 1);
        zb.color = new Color(0, 0, z.z, 1);

        wr.color = new Color(w.x,0, 0, 1);
        wg.color = new Color(0, w.y,0, 1);
        wb.color = new Color(0, 0, w.z, 1);

        /*
        debug.text = "rotation: " + rot.ToString() + "\n";
        debug.text += "angles: " + rot.eulerAngles.ToString() + "\n";
        
        debug.text += "r: " + x.ToString() + "\n";
        debug.text += "g: " + y.ToString() + "\n";
        debug.text += "b: " + z.ToString() + "\n";
        debug.text += "a: " + w.ToString();
        */

    }

    // convert rotation in Quaternion to RGB values
    // RGB -> [0, 1]
    // rotation -> [-1, 1]
    // vec = 2 * RGB - 1
    // RGB = 0.5 (vec + 1)
    Color RotationToRGB(Quaternion rotation)
    {

        Color color = new Color();
        float x = 0.5f * (rotation.x + 1);
        float x0 = ((int)(x * 100.0f)) / 100.0f;
        float x1 = x * 100.0f - x0 * 100.0f;

        color.r = x0;
        color.g = x1;
        color.b = 0;
        color.a = 1;

        return color;
    }

    Vector3 RotationToColor(float rot)
    {
        // shift the range
        float val = 0.5f * (rot + 1.0f);

        //float val = rot;

        // tenth place
        float x0 = ((int)(val * 10.0f)) / 10f;

        // hundredth place
        float x1 = (val * 10f - x0 * 10f);
        x1 = ((int)(x1 * 10f)) / 10f;

        // thousandth place
        float x2 = (val * 10.0f - x0 * 10.0f - x1) * 10.0f;
        x2 = ((int)(x2 * 10.0f + 0.5f)) / 10.0f;

        //return new Color(r: x0, g: x1, b: x2, a: 1);
        return new Vector3(x0, x1, x2);
        
    }

}
