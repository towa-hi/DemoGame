using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelection : MonoBehaviour
{
    [SerializeField] public UIMenuManager menuManager;
    
    public Transform characterList;
    public GameObject characterEntryPrefab;
    
    [SerializeField] Button backButton;
    [SerializeField] Button playButton;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buttonClip;
    
    public Image selectedCharacterImage;
    public TextMeshProUGUI selectedCharacterName;

    CharacterSelectionState state;

    
    void Start()
    {
        Reset();
    }

    public void Reset()
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
        audioSource.PlayOneShot(buttonClip);
        menuManager.StartGame();
    }

    public void OnBackButtonClicked()
    {
        audioSource.PlayOneShot(buttonClip);
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

    void FinishSelectionAndStartGame()
    {
        if (state.selectedCharacter == null)
        {
            throw new Exception("characterData was not selected");
        }
        GameManager.instance.StartGame(state);
    }
}
