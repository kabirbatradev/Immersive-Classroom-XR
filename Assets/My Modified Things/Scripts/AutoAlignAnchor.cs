using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class AutoAlignAnchor : MonoBehaviour
{

    private bool fromCloud = false;
    private bool hasAutoAligned = false;

    private OVRSpatialAnchor anchorData;

    public void SetFromCloudTrue() {
        SampleController.Instance.Log("this anchor was shared with me so it should be invisible");
        fromCloud = true;
    }

    void Start() {
        anchorData = GetComponent<OVRSpatialAnchor>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (hasAutoAligned) {
            Destroy(this); // this component no longer needs to exist
            // return;
        }

        // if the anchor is localized, that means it has been initialized and now has a pose
        if (anchorData.Localized) {

            // trigger alignment
            SampleController.Instance.Log("auto aligning...");
            GetComponent<SharedAnchor>().OnAlignButtonPressed();

            // we could even make the anchor invisible once its job is done (only if not local anchor created by admin)
            if (fromCloud) {
                gameObject.SetActive(false);
            }

            hasAutoAligned = true;
        }
    }
}
