using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
using TMPro;

public class GUIManager : MonoBehaviour
{


    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject lobbyPanel;

    [SerializeField]
    private GameObject consolePanel;

    [SerializeField]
    private GameObject roomInfoPanel;

    [SerializeField]
    private GameObject[] adminButtons;

    [SerializeField]
    private GameObject[] studentButtons;

    [SerializeField]
    private GameObject[] cameraButtons;


    private bool studentViewEnabled = false;

    [SerializeField]
    private TextMeshProUGUI studentViewButtonLabel;

    [SerializeField]
    private GameObject colorsCanvas;

    // needed to call functions SetDeviceMode...
    [SerializeField]
    private SharedAnchorControlPanelAdditionalFunctions additionalFunctionsScript;



    // we need the instance so that we can default to StudentMode from StreamlineManager when a room is joined
    public static GUIManager Instance;
    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }



    public void OnToggleStudentViewButtonPressed() {
        SampleController.Instance.Log("OnToggleStudentViewButtonPressed");
        studentViewEnabled = !studentViewEnabled;
        colorsCanvas.SetActive(studentViewEnabled);

        studentViewButtonLabel.text = studentViewEnabled ? "Disable Student View" : "Enable Student View";
    }




    public void OnExitRoomButtonPressed() {
        SampleController.Instance.Log("OnExitRoomButtonPressed");
        StreamlineManager.Instance.SetAutoJoinRoom(false);
        PhotonPun.PhotonNetwork.LeaveRoom(); 
        ResetControlPanel();
    }


    public void ResetControlPanel() {
        // OnStudentMode(); // reset main panel to student view so that the next room we join, we are in student mode by default
        lobbyPanel.SetActive(true);
        menuPanel.SetActive(false);
        consolePanel.SetActive(true);
    }


    public void OnStudentMode() {
        // display only the student buttons
        SetAdminButtonsActive(false);
        SetCameraButtonsActive(false);
        SetStudentButtonsActive(true);

        // disable the Console object
        consolePanel.SetActive(false);

        int currentGroupNumber = GetCurrentGroupNumber();
        if (currentGroupNumber <= 0) SetGroupNumber(1);

        roomInfoPanel.SetActive(false);

        additionalFunctionsScript.SetDeviceModeStudent();
    }

    public void OnAdminMode() {
        // show the admin buttons
        SetCameraButtonsActive(false);
        SetStudentButtonsActive(false);
        SetAdminButtonsActive(true);

        // enable the console object
        consolePanel.SetActive(true);
        // SetGroupNumber(1);

        roomInfoPanel.SetActive(true);

        additionalFunctionsScript.SetDeviceModeAdmin();
    }

    public void OnCameraMode() {
        // only show the camera buttons
        SetStudentButtonsActive(false);
        SetAdminButtonsActive(false);
        SetCameraButtonsActive(true);

        // disable the console object
        consolePanel.SetActive(false);
        SetGroupNumber(0);

        roomInfoPanel.SetActive(false);

        additionalFunctionsScript.SetDeviceModeCamera();
    }

    private void SetStudentButtonsActive(bool active) {
        foreach (GameObject b in studentButtons) {
            b.SetActive(active);
        }
    }
    private void SetAdminButtonsActive(bool active) {
        foreach (GameObject b in adminButtons) {
            b.SetActive(active);
        }
    }
    private void SetCameraButtonsActive(bool active) {
        foreach (GameObject b in cameraButtons) {
            b.SetActive(active);
        }
    }



    

    



    private void SetGroupNumber(int groupNumber) {
        PhotonRealtime.Player LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;
        LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
        // also set locally for faster updates
        LocalPlayer.CustomProperties["groupNumber"] = groupNumber;
    }

    private int GetCurrentGroupNumber() {
        ExitGames.Client.Photon.Hashtable PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;
        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int currentUserGroupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : 1;
        return currentUserGroupNumber;
    }


}
