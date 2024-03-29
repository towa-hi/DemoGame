using System.Collections;
using System.Collections.Generic;
using System.Net;
using Lean.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Dictionary<string, CharacterData> characterData;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (this != instance)
            {
                Destroy(gameObject);
            }
        }
        characterData = Utilities.GetCharacterDataFromJson();
        Debug.Log("GameManager loaded characterData");
    }

    public void StartGame(CharacterSelectionState characterSelectionState)
    {
        StartCoroutine(StartGameCoroutine(characterSelectionState));
    }

    IEnumerator StartGameCoroutine(CharacterSelectionState characterSelectionState)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        HUDController hud = FindObjectOfType<HUDController>(); // Replace GUI with your actual GUI component type
        if (hud != null)
        {
            hud.SetImage(characterSelectionState.selectedCharacter.GetSprite());
        }
    }

    public void ChangeLanguage(string language)
    {
        LeanLocalization.SetCurrentLanguageAll(language);
    }
}

[System.Serializable]
public class CharacterData
{
    public string id;
    public string name;
    public string spritePath;
    public int spriteIndex;

    public CharacterData(string id, string name, string spritePath, int spriteIndex)
    {
        this.id = id;
        this.name = name;
        this.spritePath = spritePath;
        this.spriteIndex = spriteIndex;
    }

    public Sprite GetSprite()
    {
        return Resources.LoadAll<Sprite>(spritePath)[spriteIndex];
    }
}
