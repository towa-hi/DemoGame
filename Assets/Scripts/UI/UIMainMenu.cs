using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] UIMenuManager menuManager;

    void Awake()
    {

    }

    public void OnBackButton()
    {
        menuManager.OpenMainMenu();
    }

    public void OnCharacterSelectButton()
    {
        menuManager.OpenCharacterSelection();
    }
}
