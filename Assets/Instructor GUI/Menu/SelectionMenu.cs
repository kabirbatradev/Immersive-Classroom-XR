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
            return;
        }

        currentMenu.SetActive(true);
        foreach (GameObject menu in otherMenu)
        {
            menu.SetActive(false);
        }
    }
}
