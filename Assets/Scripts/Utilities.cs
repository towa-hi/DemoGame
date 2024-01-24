using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using SimpleJSON;
public class Utilities
{
    public static Dictionary<string, CharacterData> GetCharacterDataFromJson()
    {
        Dictionary<string, CharacterData> characterDictionary = new Dictionary<string, CharacterData>();
        
        TextAsset textAsset = Resources.Load<TextAsset>("Data/CharacterData");
        if (textAsset == null)
        {
            Debug.LogError("CharacterData.json not found in Resources/Data/");
            return characterDictionary;
        }

        string dataString = textAsset.text;
        var n = JSON.Parse(dataString);
        foreach ((string key, JSONNode value) in n)
        {
            CharacterData characterData = new CharacterData(key, value["name"], value["sprite_path"], value["sprite_index"].AsInt);
            characterDictionary[key] = characterData;
        }
        return characterDictionary;
    }
}
[System.Serializable]
public class CharacterDataContainer
{
    public Dictionary<string, CharacterData> characterData = new Dictionary<string, CharacterData>();
}

public enum ButtonStatus
{
    UNSELECTED,
    HOVERED,
    SELECTED
}
