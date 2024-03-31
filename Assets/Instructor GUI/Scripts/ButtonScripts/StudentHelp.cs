using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StudentHelp : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int maxGroup = 40;

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { OnDropdownChanged(dropdown.value); });
    }
    void Update()
    {
        //maxGroup = InstructorCloudFunctions.Instance.GetMaxGroupNumber();
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 1; i <= maxGroup; i++)
        {
            options.Add("Group: " + i.ToString());
        }
        dropdown.AddOptions(options);
    }

    void OnDropdownChanged(int index)
    {
        switch (index)
        {
            case 0:
                Debug.Log("Option 1 selected");
                break;
            case 1:
                Debug.Log("Option 2 selected");
                break;
            case 2:
                Debug.Log("Option 3 selected");
                break;
            default:
                Debug.Log("Unknown option");
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
