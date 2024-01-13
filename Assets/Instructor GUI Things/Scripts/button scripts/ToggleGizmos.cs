using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using RTG; // might be required for using RuntimeGizmo class

public class ToggleGizmos : MonoBehaviour
{

    private bool gizmosAreEnabled = false;

    public void Start() {
        RuntimeGizmo.ToggleGizmo(gizmosAreEnabled);
        Laser.ToggleLaser(!gizmosAreEnabled);
    }

    public void toggleGizmos() {

        gizmosAreEnabled = !gizmosAreEnabled;

        RuntimeGizmo.ToggleGizmo(gizmosAreEnabled);
        Laser.ToggleLaser(!gizmosAreEnabled);
    }

}
