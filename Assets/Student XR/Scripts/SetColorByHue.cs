using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColorByHue : MonoBehaviour
{
    private Renderer headRenderer;

    [SerializeField]
    private float hue;

    // Start is called before the first frame update
    void Start()
    {
        headRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        headRenderer.material.color = Color.HSVToRGB(hue, 1.0f, 1.0f);
    }
}
