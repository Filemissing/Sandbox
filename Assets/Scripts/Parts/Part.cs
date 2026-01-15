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

        Transform originalParent;
        Vector3Int originalPosition;
        public bool isOnField;
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;
            originalPosition = Vector3Int.RoundToInt(transform.position);
            transform.SetParent(null);

            if (isOnField) BuildField.instance?.RemovePart(this, new Vector2Int(originalPosition.x, originalPosition.y));
            else PartLibrary.instance.RemovePart(this);

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
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                if (collider is WheelCollider) continue; // wheel colliders should remain disabled during placement
                collider.enabled = true;
            }

            // if a drop handler accepted the part we are done
            if (eventData.used) return;

            // failed placement - restore original field state
            if (originalParent == BuildField.instance.robotParent)
            {
                // attempt to re-place the part at original grid position
                Vector2Int gridPos = new Vector2Int(originalPosition.x, originalPosition.y);

                if (BuildField.instance.TryAddPart(this, gridPos))
                {
                    isOnField = true;

                    transform.SetParent(originalParent);
                    transform.position = originalPosition;
                    return;
                }
            }
            PartLibrary.instance.ReturnPart(this); // fallback to part library
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
