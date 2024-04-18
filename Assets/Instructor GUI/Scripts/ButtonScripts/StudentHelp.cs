using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class StudentHelp : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private List<int> currentGroupNumbers = new List<int>();

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        UpdateDropdownOptions();
    }

    void Update()
    {
        var newGroupNumbers = InstructorCloudFunctions.Instance.GetGroupsRequestingHelp();
        if (!Enumerable.SequenceEqual(newGroupNumbers, currentGroupNumbers))
        {
            UpdateDropdownOptions();
        }
    }

    void UpdateDropdownOptions()
    {
        currentGroupNumbers = InstructorCloudFunctions.Instance.GetGroupsRequestingHelp();
        dropdown.ClearOptions();
        List<string> options = new List<string> { "No Group" };
        options.Add("All Groups");
        foreach (var groupNumber in currentGroupNumbers)
        {
            options.Add($"Group {groupNumber}");
        }
        dropdown.AddOptions(options);
    }

    void OnDropdownChanged(int index)
    {
        // "None" option selected
        if (index == 0)
        {
            Debug.Log("Setting current group to -1");
            InstructorCloudFunctions.Instance.SetInstructorPanelCurrentGroup(-1);
        }
        else if (index == 1)
        {
            Debug.Log("Setting current group to 0");
            InstructorCloudFunctions.Instance.SetInstructorPanelCurrentGroup(0);
        }
        else
        {
            int actualGroupNumber = currentGroupNumbers[index - 1];
            Debug.Log($"Setting current group to {actualGroupNumber}");
            InstructorCloudFunctions.Instance.SetInstructorPanelCurrentGroup(actualGroupNumber);
        }
    }

    void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }
}
