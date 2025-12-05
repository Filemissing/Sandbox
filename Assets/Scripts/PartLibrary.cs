using UnityEngine;
using UnityEngine.EventSystems;

namespace RobotGame
{
    public class PartLibrary : MonoBehaviour, IDropHandler
    {
        public static PartLibrary instance;

        private void Awake()
        {
            instance = this;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            if (eventData.pointerDrag.TryGetComponent<Part>(out Part part))
            {
                // if the part is currently on the field remove it safely
                if (part.isOnField)
                {
                    Vector3Int pos = Vector3Int.RoundToInt(part.transform.position);
                    BuildField.instance.RemovePart(part, new Vector2Int(pos.x, pos.y));
                }

                // reset transform under the library
                part.transform.SetParent(transform);
                part.transform.localPosition = Vector3.zero;

                // reset root and flags
                part.rootBlock = null;
                part.isOnField = false;

                eventData.Use();
            }
        }
    }
}
