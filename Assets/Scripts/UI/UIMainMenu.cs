using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenu : UIMenuPanel
{
    [SerializeField] UIMenuManager menuManager;

    public void OnQuitButton()
    {
        menuManager.OpenMainMenu();
    }

    public void OnCharacterSelectButton()
    {
        menuManager.OpenCharacterSelection();
    }

    public void OnLanguageENButton()
    {
        GameManager.instance.ChangeLanguage("English");
    }

    public void OnLanguageJPButton()
    {
        GameManager.instance.ChangeLanguage("Japanese");
    }
}
