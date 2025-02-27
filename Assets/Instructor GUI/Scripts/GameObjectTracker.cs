using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
[Serializable]
public class GameObjectData
{
    public string name;
    public string tag;
    public Vector3 position;
    public Quaternion rotation;

    public int groupNumber;

    // optional bounding box data (the player heads dont need this, but the main objects and panels should)
    public Vector3 center;
    public Vector3 size;
}

[Serializable]
public class FrameData
{
    public int frameNumber;
    public List<GameObjectData> gameObjects = new List<GameObjectData>();

    // record the current group mode (lecture mode vs groups of 4 etc)
    public string currentGroupMode;

    // record the currently active game object
    public string currentMainObjectModelName;

    // theater related data:
    // StreamTheaterModeData.Instance should constantly have these up to date
    public float wallLoweredPercentage;
    public float ceilingRemovedPercentage;
    public bool ceilingVisible;
    public int currentSkyboxIndex;
    public string skyboxMaterialName;

}

[Serializable]
public class TrackedData
{
    public List<FrameData> frames = new List<FrameData>();
}

public class GameObjectTracker : MonoBehaviour
{
    public List<GameObject> gameObjectsToTrack = new List<GameObject>();
    private TrackedData trackedData = new TrackedData();
    public float recordFrequency = 1.0f;
    private string path;

    private void Start()
    {
        string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        path = Application.dataPath + "/TrackedData/TrackedData" + time + ".json";
        if (!Directory.Exists(Application.dataPath + "/TrackedData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/TrackedData");
        }
        StartCoroutine(RecordData());
    }

    private IEnumerator RecordData()
    {
        Debug.Log("RecordData calls called");
        while (true)
        {
            Debug.Log("Recording data...");
            FrameData frameData = new FrameData();
            frameData.frameNumber = Time.frameCount;
            frameData.currentGroupMode = InstructorCloudFunctions.Instance.currentGroupMode.ToString();

            bool youSuck = false;

            try
            {

                if (InstructorCloudFunctions.Instance.RoomHasCustomProperty("mainObjectCurrentModelName"))
                {
                    Debug.Log("has custom property = " + InstructorCloudFunctions.Instance.RoomHasCustomProperty("mainObjectCurrentModelName"));
                    frameData.currentMainObjectModelName = (string)InstructorCloudFunctions.Instance.GetRoomCustomProperty("mainObjectCurrentModelName");
                }
                else
                {
                    Debug.Log("has custom property = " + InstructorCloudFunctions.Instance.RoomHasCustomProperty("mainObjectCurrentModelName"));
                    frameData.currentMainObjectModelName = "No Main Object Model";
                }

            }
            catch (Exception e)
            {
                Debug.Log("not connected to server; not logging data on this iteration");
                youSuck = true;
            }

            if (youSuck)
            {
                yield return new WaitForSeconds(1f / recordFrequency); // should restart the while loop 
                continue;
            }



            frameData.wallLoweredPercentage = StreamTheaterModeData.Instance.wallLoweredPercentage;
            frameData.ceilingRemovedPercentage = StreamTheaterModeData.Instance.ceilingRemovedPercentage;
            frameData.ceilingVisible = StreamTheaterModeData.Instance.ceilingVisible;
            frameData.currentSkyboxIndex = StreamTheaterModeData.Instance.currentSkyboxIndex;
            frameData.skyboxMaterialName = StreamTheaterModeData.Instance.skyboxList[StreamTheaterModeData.Instance.currentSkyboxIndex].ToString();
            // StreamTheaterModeData.Instance.skyboxList[]

            // iterate through all player heads, get student username and group number
            GameObject[] studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
            foreach (GameObject studentHead in studentsHeads)
            {
                if (studentHead == null) continue;

                Player studentPlayer = InstructorCloudFunctions.Instance.GetPlayerFromPlayerHeadObject(studentHead);
                int studentGroupNumber = InstructorCloudFunctions.Instance.GetPlayerGroupNumber(studentPlayer);
                string studentName = studentPlayer.NickName;

                GameObjectData data = new GameObjectData
                {
                    name = studentHead.name + " " + studentName,
                    tag = studentHead.tag,
                    position = studentHead.transform.position,
                    rotation = studentHead.transform.rotation,
                    groupNumber = studentGroupNumber,
                };
                frameData.gameObjects.Add(data);
            }


            // iterate though all main objects, get group number, add bounds
            GameObject[] mainObjects = GameObject.FindGameObjectsWithTag("MainObjectContainer");
            foreach (GameObject mainObject in mainObjects)
            {
                if (mainObject == null) continue;

                int objectGroupNumber = 1;
                if (InstructorCloudFunctions.Instance.PhotonObjectHasGroupNumber(mainObject))
                {
                    objectGroupNumber = InstructorCloudFunctions.Instance.GetPhotonObjectGroupNumber(mainObject);
                }

                // get the subobject currently being rendered
                Bounds worldCoordinateBounds = new(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
                bool activeMainObjectModel = false;
                foreach (Transform mainObjectModel in mainObject.transform)
                {
                    if (mainObjectModel.gameObject.activeSelf)
                    {
                        worldCoordinateBounds = mainObjectModel.GetComponent<Renderer>().bounds;
                        activeMainObjectModel = true;
                        break;
                    }
                }

                // if there is somehow no active model
                if (!activeMainObjectModel)
                {
                    Debug.Log("ERROR: NO ACTIVE MAIN OBJECT MODEL FOUND");
                    continue;
                }

                GameObjectData data = new GameObjectData
                {
                    name = mainObject.name,
                    tag = mainObject.tag,
                    position = mainObject.transform.position,
                    rotation = mainObject.transform.rotation,
                    groupNumber = objectGroupNumber,

                    center = worldCoordinateBounds.center,
                    size = worldCoordinateBounds.size,

                };

                frameData.gameObjects.Add(data);
            }

            // iterate through all panels, 
            // separate into the actual panel and the professor head, 
            // get group number, add bounds bounds
            GameObject[] sidePanelObjects = GameObject.FindGameObjectsWithTag("SidePanel");
            foreach (GameObject sidePanelObject in sidePanelObjects)
            {
                if (sidePanelObject == null) continue;

                int objectGroupNumber = 1;
                if (InstructorCloudFunctions.Instance.PhotonObjectHasGroupNumber(sidePanelObject))
                {
                    objectGroupNumber = InstructorCloudFunctions.Instance.GetPhotonObjectGroupNumber(sidePanelObject);
                }

                // every side panel has 2 child objects (the quiz panel and the video panel)
                GameObject quizPanelObject = sidePanelObject.transform.GetChild(0).gameObject;

                Bounds worldCoordinateBounds = quizPanelObject.GetComponent<Renderer>().bounds;

                GameObjectData data = new GameObjectData
                {
                    name = quizPanelObject.name,
                    tag = quizPanelObject.tag,
                    position = quizPanelObject.transform.position,
                    rotation = quizPanelObject.transform.rotation,
                    groupNumber = objectGroupNumber,

                    center = worldCoordinateBounds.center,
                    size = worldCoordinateBounds.size,
                };
                frameData.gameObjects.Add(data);

                // if the agora panel doesnt exist, then dont try to log it
                if (sidePanelObject.transform.childCount != 2) continue;
                GameObject agoraVideoPanel = sidePanelObject.transform.GetChild(1).gameObject;

                worldCoordinateBounds = agoraVideoPanel.GetComponent<Renderer>().bounds;

                data = new GameObjectData
                {
                    name = agoraVideoPanel.name,
                    tag = agoraVideoPanel.tag,
                    position = agoraVideoPanel.transform.position,
                    rotation = agoraVideoPanel.transform.rotation,
                    groupNumber = objectGroupNumber,

                    center = worldCoordinateBounds.center,
                    size = worldCoordinateBounds.size,
                };
                frameData.gameObjects.Add(data);

            }


            // foreach (GameObject student in studentsHeads)
            // {
            //     AddGameObject(student);
            // }
            // foreach (GameObject mainObject in mainObjects)
            // {
            //     AddGameObject(mainObject);
            // }
            // foreach (GameObject obj in gameObjectsToTrack)
            // {
            //     if (obj != null)
            //     {
            //         GameObjectData data = new GameObjectData
            //         {
            //             name = obj.name,
            //             tag = obj.tag,
            //             position = obj.transform.position,
            //             rotation = obj.transform.rotation
            //         };
            //         frameData.gameObjects.Add(data);
            //     }
            // }
            trackedData.frames.Add(frameData);
            ExportDataToJson();  // Consider moving this outside the loop if only needed at end of session
            yield return new WaitForSeconds(1f / recordFrequency);
        }
    }

    // public void AddGameObject(GameObject obj)
    // {
    //     if (obj != null && !gameObjectsToTrack.Contains(obj))
    //     {
    //         gameObjectsToTrack.Add(obj);
    //         Debug.Log("Added GameObject: " + obj.name);
    //     }
    // }

    public void RemoveGameObject(GameObject obj)
    {
        if (gameObjectsToTrack.Contains(obj))
        {
            gameObjectsToTrack.Remove(obj);
        }
    }

    public void ExportDataToJson()
    {
        string json = JsonUtility.ToJson(trackedData, true);
        File.WriteAllText(path, json);
        Debug.Log("Data exported to JSON file at: " + path);
    }
}
