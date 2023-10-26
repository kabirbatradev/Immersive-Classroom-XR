using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentData : MonoBehaviour
{
    // Start is called before the first frame update

    public int groupNumber = 0;
    public StudentData() {
        groupNumber = 0;
    }
    // void Start()
    // {
    //     groupNumber = 0;
    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void SetGroupNumber(int groupNum) {
        groupNumber = groupNum;
    }
}
