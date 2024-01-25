using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuManager : MonoBehaviour
{
    [Header("REQUIRED")]
    [SerializeField] UIMainMenu mainMenuPanel;
    [SerializeField] UICharacterSelection characterSelectionPanel;
    [Header("STATE")]
    [SerializeField] UIMenuPanel currentPanel;
    [SerializeField] List<UIMenuPanel> allPanels;
    
    void Awake()
    {
        allPanels = new List<UIMenuPanel>();
        UIMenuPanel[] panels = GetComponentsInChildren<UIMenuPanel>();
        foreach (UIMenuPanel panel in panels)
        {
            allPanels.Add(panel);
        }
        SetCurrentPanel(mainMenuPanel);
    }

    void SetCurrentPanel(UIMenuPanel newPanel)
    {
        if (currentPanel)
        {
            currentPanel.Activate(false);
        }
        currentPanel = newPanel;
        newPanel.Activate(true);

    }
    
    public void OpenCharacterSelection()
    {
        SetCurrentPanel(characterSelectionPanel);
    }

    public void OpenMainMenu()
    {
        SetCurrentPanel(mainMenuPanel);
    }
}
