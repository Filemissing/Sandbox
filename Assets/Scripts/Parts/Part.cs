using UnityEngine;
using UnityEngine.EventSystems;

namespace RobotGame
{
    public class Part : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public float weight;
        public float hp;
        public RectInt space;

        public Block rootBlock; // only applies to non-block parts

        public void Detatch()
        {
            transform.SetParent(null);
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = weight;
        }


        public float zOffset;

        bool isDragging;
        Transform originalParent;
        Vector3Int originalPosition;
        /*[HideInInspector]*/ public bool isOnField;
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            originalParent = transform.parent;
            originalPosition = Vector3Int.RoundToInt(transform.position);
            transform.SetParent(null);

            if (isOnField) BuildField.instance?.RemovePart(this, new Vector2Int(originalPosition.x, originalPosition.y));

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 position = GetWorldPositionOnPlane(eventData.position, zOffset);
            transform.position = Vector3Int.RoundToInt(position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }

            if (eventData.used) return;
            transform.SetParent(originalParent);
            transform.position = originalPosition;
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
