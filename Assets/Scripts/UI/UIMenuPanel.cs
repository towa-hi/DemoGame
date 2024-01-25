using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuPanel : MonoBehaviour
{
    public bool isActivated;
    public virtual void Activate(bool isActivated)
    {
        this.isActivated = isActivated;
        gameObject.SetActive(isActivated);
    }
}
