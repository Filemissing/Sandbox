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
            Vector3 position = GetWorldPositionOnPlane(eventData.position, zOffset);
            transform.position = Vector3Int.RoundToInt(position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            if (eventData.used) return;
            transform.SetParent(originalParent);
        }

        public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
            float distance;
            xy.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    } 
}
