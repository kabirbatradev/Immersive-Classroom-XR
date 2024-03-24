using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToTheaterManager : MonoBehaviour
{
    void Start() {
        TheaterModeManager.Instance.AddScenePlane(gameObject);
    }
}
