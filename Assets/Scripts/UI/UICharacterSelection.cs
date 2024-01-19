using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelection : MonoBehaviour
{
    public Transform characterList;
    public GameObject characterEntryPrefab;
    
    [SerializeField] Button backButton;
    [SerializeField] Button playButton;
    
    public Image selectedCharacterImage;
    public TextMeshProUGUI selectedCharacterName;

    CharacterSelectionState state;

    
    void Start()
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
    
    public void OnEntryClicked(UICharacterEntry entry)
    {
        SelectCharacter(entry.characterData);
    }

    public void OnStartButtonClicked()
    {
        FinishSelectionAndStartGame();
    }

    public void OnBackButtonClicked()
    {
        // return to prev screen
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

    void FinishSelectionAndStartGame()
    {
        if (state.selectedCharacter == null)
        {
            throw new Exception("characterData was not selected");
        }
        GameManager.instance.StartGame(state);
    }
}
