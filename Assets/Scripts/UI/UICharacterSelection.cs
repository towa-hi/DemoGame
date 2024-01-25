using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelection : UIMenuPanel
{
    [Header("REQUIRED")]
    [SerializeField] UIMenuManager menuManager;
    [SerializeField] Transform characterList;
    [SerializeField] GameObject characterEntryPrefab;
    [SerializeField] Button backButton;
    [SerializeField] Button playButton;
    [SerializeField] Image selectedCharacterImage;
    [SerializeField] TextMeshProUGUI selectedCharacterName;
    // state
    CharacterSelectionState state;

    public override void Activate(bool isActivated)
    {
        base.Activate(isActivated);
        if (isActivated)
        {
            Reset();
        }
    }

    void Reset()
    {
        state = new CharacterSelectionState();
        InitializeCharacterList();
        RefreshView();
    }
    
    void OnDestroy()
    {
        // unsub entries (they should be getting deleted anyways)
        foreach (Transform child in characterList)
        {
            UICharacterEntry entryComponent = child.GetComponent<UICharacterEntry>();
            if (entryComponent != null)
            {
                entryComponent.onEntryClicked -= OnEntryClicked;
            }
        }
    }

    void InitializeCharacterList()
    {
        foreach (Transform child in characterList)
        {
            if (child.GetComponent<UICharacterEntry>())
            {
                Destroy(child.gameObject);
            }
            
        }
        foreach (CharacterData characterData in GameManager.instance.characterData.Values)
        {
            GameObject currentEntry = Instantiate(characterEntryPrefab, characterList);
            currentEntry.GetComponent<UICharacterEntry>().Initialize(OnEntryClicked, characterData);
        }
    }
    
    void OnEntryClicked(UICharacterEntry entry)
    {
        SelectCharacter(entry.characterData);
    }

    public void OnStartButtonClicked()
    {
        GameManager.instance.StartGame(state);
    }

    public void OnBackButtonClicked()
    {
        menuManager.OpenMainMenu();
    }
    
    void SelectCharacter(CharacterData characterData)
    {
        state.SetSelectedCharacter(characterData);
        RefreshView();
    }

    void RefreshView()
    {
        // set entries
        foreach (Transform child in characterList)
        {
            UICharacterEntry currentEntry = child.GetComponent<UICharacterEntry>();
            currentEntry.SelectEntry(currentEntry.characterData == state.selectedCharacter);
        }
        // if character selected
        if (state.selectedCharacter != null)
        {
            selectedCharacterImage.sprite = state.selectedCharacter.GetSprite();
            selectedCharacterImage.enabled = true;
            selectedCharacterName.text = state.selectedCharacter.name;
            playButton.interactable = true;
        }
        else
        {
            selectedCharacterImage.sprite = null;
            selectedCharacterImage.enabled = false;
            selectedCharacterName.text = "";
            playButton.interactable = false;
        }
    }

}
