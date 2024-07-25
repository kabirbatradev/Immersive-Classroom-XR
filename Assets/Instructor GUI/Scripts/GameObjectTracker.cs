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

    // add optional bounding box data (the player heads dont need this, but the main objects and panels should)
}

[Serializable]
public class FrameData
{
    public int frameNumber;
    public List<GameObjectData> gameObjects = new List<GameObjectData>();

    // TODO: record the current main object mode (lecture mode vs groups of 4 etc)
    // TODO: record the current ... something else...
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
        while (true)
        {
            FrameData frameData = new FrameData();
            frameData.frameNumber = Time.frameCount;

            // iterate through all player heads, get student username and group number
            GameObject[] studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
            foreach (GameObject studentHead in studentsHeads) {
                if (studentHead == null) continue;

                Player studentPlayer = InstructorCloudFunctions.Instance.GetPlayerFromPlayerHeadObject(studentHead);
                int studentGroupNumber = InstructorCloudFunctions.Instance.GetPlayerGroupNumber(studentPlayer);
                string studentName = studentPlayer.NickName;

                GameObjectData data = new GameObjectData {
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
            foreach (GameObject mainObject in mainObjects) {
                if (mainObject == null) continue;

                int objectGroupNumber = 1;
                if (InstructorCloudFunctions.Instance.PhotonObjectHasGroupNumber(mainObject)) {
                    objectGroupNumber = InstructorCloudFunctions.Instance.GetPhotonObjectGroupNumber(mainObject);
                }

                GameObjectData data = new GameObjectData {
                    name = mainObject.name,
                    tag = mainObject.tag,
                    position = mainObject.transform.position,
                    rotation = mainObject.transform.rotation,
                    groupNumber = objectGroupNumber,
                };
                frameData.gameObjects.Add(data);
            }

            // TODO: iterate through all panels, 
            // separate into the actual panel and the professor head, 
            // get group number, add bounds bounds



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
