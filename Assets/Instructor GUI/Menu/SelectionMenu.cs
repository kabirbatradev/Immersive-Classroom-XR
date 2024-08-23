using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionMenu : MonoBehaviour
{
    public GameObject currentMenu;
    public GameObject[] otherMenu;

    public void ChangeToMenu()
    {
        if (currentMenu.activeSelf)
        {
            currentMenu.SetActive(false);
            Debug.Log("Changed to menu: " + currentMenu.name);
            return;
        }

        currentMenu.SetActive(true);
        foreach (GameObject menu in otherMenu)
        {
            Debug.Log("Deactivated menu: " + menu.name);
            menu.SetActive(false);
        }
    }
}
