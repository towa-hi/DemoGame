using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] UIMainMenu mainMenu;
    [SerializeField] UICharacterSelection characterSelection;
    void Awake()
    {
        mainMenu.gameObject.SetActive(true);
    }

    public void OpenCharacterSelection()
    {
        mainMenu.gameObject.SetActive(false);
        characterSelection.gameObject.SetActive(true);
        characterSelection.Reset();
    }

    public void OpenMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        characterSelection.gameObject.SetActive(false);
    }
}
