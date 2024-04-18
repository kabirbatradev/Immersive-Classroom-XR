using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{

    public int groupNumber = 0;
    private Renderer headSphereRenderer;
    // Start is called before the first frame update
    void Start()
    {
        headSphereRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // when mouse is pressed, increment group number
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame) {
            groupNumber++;
        }


        float hue = (groupNumber % 6) / 6.0f; // max group number is 6; if more than 6, then the color cycles back
        // i chose 6 because the spacing between colors is really nice!

        // renderer.material.color = Color.HSVToRGB(0.5f, 1.0f, 1.0f); // this does work yay
        Color c = Color.HSVToRGB(hue, 1.0f, 1.0f);
        // set the color alpha to 0.3
        c.a = 0.3f;
        headSphereRenderer.material.color = c;
    }
}
