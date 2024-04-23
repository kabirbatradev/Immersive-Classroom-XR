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
}

[Serializable]
public class FrameData
{
    public int frameNumber;
    public List<GameObjectData> gameObjects = new List<GameObjectData>();
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
            GameObject[] studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
            GameObject[] mainObjects = GameObject.FindGameObjectsWithTag("MainObjectContainer");
            foreach (GameObject student in studentsHeads)
            {
                AddGameObject(student);
            }
            foreach (GameObject mainObject in mainObjects)
            {
                AddGameObject(mainObject);
            }
            foreach (GameObject obj in gameObjectsToTrack)
            {
                if (obj != null)
                {
                    GameObjectData data = new GameObjectData
                    {
                        name = obj.name,
                        tag = obj.tag,
                        position = obj.transform.position,
                        rotation = obj.transform.rotation
                    };
                    frameData.gameObjects.Add(data);
                }
            }
            trackedData.frames.Add(frameData);
            ExportDataToJson();  // Consider moving this outside the loop if only needed at end of session
            yield return new WaitForSeconds(1f / recordFrequency);
        }
    }

    public void AddGameObject(GameObject obj)
    {
        if (obj != null && !gameObjectsToTrack.Contains(obj))
        {
            gameObjectsToTrack.Add(obj);
            Debug.Log("Added GameObject: " + obj.name);
        }
    }

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
