using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharacterEntry : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] Button button;
    UICharacterSelection characterSelection;
    public CharacterData characterData;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] AudioClip hoverClip;
    [SerializeField] AudioClip selectClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Image background;
    
    public event Action<UICharacterEntry> onEntryClicked;

    public bool isSelected = false;
    
    public void Initialize(Action<UICharacterEntry> onEntryClicked, CharacterData characterData)
    {
        this.onEntryClicked = onEntryClicked;
        this.characterData = characterData;
        characterName.text = this.characterData.name;
        characterImage.sprite = Resources.LoadAll<Sprite>(characterData.spritePath)[characterData.spriteIndex];
    }

    public void OnClick()
    {
        onEntryClicked?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            audioSource.PlayOneShot(hoverClip, 0.2f);
        }
    }
    
    public void SelectEntry(bool selected)
    {
        isSelected = selected;
        button.interactable = !selected;
        if (selected)
        {
            audioSource.PlayOneShot(selectClip);
        }
    }
}
