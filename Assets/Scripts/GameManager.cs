using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Dictionary<string, CharacterData> characterData;
    
    public PlayerController playerController;
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
    void Start()
    {

    }


    public static List<CharacterData> GetCharacterDataFromJson()
    {
        List<CharacterData> characterList = new List<CharacterData>();
        
        return characterList;
    }

    public void StartGame(CharacterSelectionState characterSelectionState)
    {
        // start game
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
