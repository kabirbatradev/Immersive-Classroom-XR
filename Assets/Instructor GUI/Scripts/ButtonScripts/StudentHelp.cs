using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StudentHelp : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(dropdown.value); });
    }
    void Update()
    {
        //maxGroup = InstructorCloudFunctions.Instance.GetMaxGroupNumber();
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        // add none option
        options.Add("None");
        List<int> groupNumbers = InstructorCloudFunctions.Instance.GetGroupsRequestingHelp();
        for (int i = 0; i < groupNumbers.Count; i++)
        {
            options.Add("Group " + groupNumbers[i]);
        }
        dropdown.AddOptions(options);
    }

    void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 0:
                InstructorCloudFunctions.Instance.SetInstructorPanelCurrentGroup(-1);
                break;
            default:
                InstructorCloudFunctions.Instance.SetInstructorPanelCurrentGroup(index);
                break;
        }
    }

    void OnDestroy()
    {
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(delegate { OnDropdownChanged(dropdown.value); });
        }
    }
}
