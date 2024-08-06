using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuestionRemoteControl : MonoBehaviour
{
    public void GradeAllQuestions()
    {
        string path = Application.dataPath + "/TrackedData/EventData.csv";
        GameObject[] sidePanels = GameObject.FindGameObjectsWithTag("SidePanel");
        foreach (GameObject panel in sidePanels)
        {
            PanelRPCFunctions panelRPC = panel.GetComponent<PanelRPCFunctions>();
            if (panelRPC != null)
            {
                panelRPC.CallRPCGradeQuestion();
            }
        }
        // save the event to a local csv file
        string content = $"Grade the current question.";
        // Get the current timestamp
        string timestamp = DateTime.UtcNow.ToString("o");
        // Create the new row
        string newRow = $"{timestamp},{content}";
        File.AppendAllText(path, newRow + Environment.NewLine);
    }

    public void NextAllQuestions()
    {
        string path = Application.dataPath + "/TrackedData/EventData.csv";
        GameObject[] sidePanels = GameObject.FindGameObjectsWithTag("SidePanel");
        foreach (GameObject panel in sidePanels)
        {
            PanelRPCFunctions panelRPC = panel.GetComponent<PanelRPCFunctions>();
            if (panelRPC != null)
            {
                panelRPC.CallRPCNextQuestion();
            }
        }
        // save the event to a local csv file
        string content = $"Next Question.";
        // Get the current timestamp
        string timestamp = DateTime.UtcNow.ToString("o");
        // Create the new row
        string newRow = $"{timestamp},{content}";
        File.AppendAllText(path, newRow + Environment.NewLine);
    }
}
