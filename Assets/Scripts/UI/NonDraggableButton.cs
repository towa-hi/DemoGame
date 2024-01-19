using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonDraggableButton : Button
{

    public override void OnPointerDown(PointerEventData eventData)
    {
        // ignore pointer down
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // ignore pointer up
    }
}
