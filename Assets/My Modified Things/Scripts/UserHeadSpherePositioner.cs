using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHeadSpherePositioner : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = transform.parent.transform.position + new Vector3(0, 0.6f, 0);

        gameObject.transform.rotation = Quaternion.identity;

    }
}
