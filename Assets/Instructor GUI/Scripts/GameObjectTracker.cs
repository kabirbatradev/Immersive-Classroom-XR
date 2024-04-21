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
    public List<Vector3> positions = new List<Vector3>();
    public List<Quaternion> rotations = new List<Quaternion>();
}

public class GameObjectTracker : MonoBehaviour
{
    public List<GameObject> gameObjectsToTrack = new List<GameObject>();
    private Dictionary<GameObject, GameObjectData> trackedData = new Dictionary<GameObject, GameObjectData>();
    public float recordFrequency = 1.0f;

    private void Start()
    {
        StartCoroutine(RecordData());
    }

    private IEnumerator RecordData()
    {
        while (true)
        {
            GameObject[] studentsHeads = GameObject.FindGameObjectsWithTag("PlayerHead");
            for (int i = 0; i < studentsHeads.Length; i++)
            {
                AddGameObject(studentsHeads[i]);
            }
            foreach (GameObject obj in gameObjectsToTrack)
            {
                if (!trackedData.ContainsKey(obj))
                {
                    trackedData[obj] = new GameObjectData();
                }

                trackedData[obj].positions.Add(obj.transform.position);
                trackedData[obj].rotations.Add(obj.transform.rotation);
            }
            ExportDataToJson();
            yield return new WaitForSeconds(1f / recordFrequency);
        }
    }

    public void AddGameObject(GameObject obj)
    {
        if (!gameObjectsToTrack.Contains(obj))
        {
            gameObjectsToTrack.Add(obj);
            if (!trackedData.ContainsKey(obj))
            {
                trackedData[obj] = new GameObjectData();
            }
        }
    }

    public void RemoveGameObject(GameObject obj)
    {
        if (gameObjectsToTrack.Contains(obj))
        {
            gameObjectsToTrack.Remove(obj);
            trackedData.Remove(obj);
        }
    }

    public void ExportDataToJson()
    {
        string json = JsonUtility.ToJson(trackedData);
        File.WriteAllText(Application.dataPath + "/TrackedData.json", json);
        Debug.Log("Data exported to JSON file at: " + Application.dataPath + "/TrackedData.json");
    }
}
