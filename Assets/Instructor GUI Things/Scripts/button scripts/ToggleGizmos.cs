using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using RTG; // might be required for using RuntimeGizmo class

public class ToggleGizmos : MonoBehaviour
{

    private bool gizmosAreEnabled = false;

    public void Start()
    {
        RuntimeGizmo.ToggleGizmo(gizmosAreEnabled);
        Laser.ToggleLaser(!gizmosAreEnabled);
        CommunicationScript.ToggleServerLaser(!gizmosAreEnabled);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            toggleGizmos();
        }
    }

    public void toggleGizmos()
    {

        gizmosAreEnabled = !gizmosAreEnabled;

        RuntimeGizmo.ToggleGizmo(gizmosAreEnabled);
        Laser.ToggleLaser(!gizmosAreEnabled);
        CommunicationScript.ToggleServerLaser(!gizmosAreEnabled);
    }

}
