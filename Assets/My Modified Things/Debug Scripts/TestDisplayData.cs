using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDisplayData : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        // press enter key to display some data
        if (Input.GetKeyDown(KeyCode.Return)) {
            DisplayData();
        }

    }

    private void DisplayData() {

        // for each marker, get its data
        SeatMarkerData[] objs = FindObjectsOfType<SeatMarkerData>();
        // GameObject[] objs = GameObject.FindGameObjectsWithTag("SeatMarker");
        int i = 0;
        foreach (SeatMarkerData s in objs) {
            Debug.Log(i + "th marker data:");
            Debug.Log(s.GetRow());
            Debug.Log(s.GetColumn());
            Debug.Log(s.GetTotalSeatsInThisRow());
            i++;
            Debug.Log("\n");
        }
        
    }
}
