using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionState
{
    public CharacterData selectedCharacter;

    public void SetSelectedCharacter(CharacterData selectedCharacter)
    {
        this.selectedCharacter = selectedCharacter;
    }
}
