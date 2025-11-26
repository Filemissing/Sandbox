using UnityEngine;
using UnityEngine.EventSystems;

namespace RobotGame
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public float zOffset;

        bool isDragging;
        Transform originalParent;
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            originalParent = transform.parent;
            transform.SetParent(null);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 roundedPosition = Vector2Int.RoundToInt(eventData.position);
            transform.position = new Vector3(roundedPosition.x, roundedPosition.y, zOffset);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            if (eventData.used) return;
            transform.SetParent(originalParent);
        }
    } 
}
