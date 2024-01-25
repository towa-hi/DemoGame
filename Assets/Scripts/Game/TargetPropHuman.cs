using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPropHuman : MonoBehaviour
{

    void OnHit(string bodyPart)
    {
        if (bodyPart == "head")
        {
            Debug.Log("hit head");
        }
        else if (bodyPart == "body")
        {
            Debug.Log("hit body");
        }
    }
}
