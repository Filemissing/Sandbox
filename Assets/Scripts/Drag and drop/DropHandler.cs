using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        bool canDrop = true;

        if (canDrop) eventData.Use(); // use if drop is allowed
    }
}
