using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectData : MonoBehaviour
{

    public int groupNumber;

    // Start is called before the first frame update
    void Start()
    {
        groupNumber = 0;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void SetGroupNumber(int groupNum) {
        groupNumber = groupNum;
    }

    
}
