using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHeadPositionTrackerManager : MonoBehaviour
{

    // this class is a singleton so that the photon prefab for the head can get the head transform data from this script easily
    public static UserHeadPositionTrackerManager Instance;

    public Transform localHeadTransform;

    private void Awake() {
        Instance = this;
    }
}
