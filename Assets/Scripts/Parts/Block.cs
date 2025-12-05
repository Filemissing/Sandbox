using UnityEngine;
using UnityEngine.EventSystems;

namespace RobotGame
{
	public class Block : Part, IDropHandler
	{
        public Part attachedPart;

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag != null)
            {
                if (eventData.pointerDrag.TryGetComponent<Part>(out Part part))
                {
                    if (BuildField.instance.TryAddPart(part, Vector2Int.RoundToInt(transform.position)))
                    {
                        attachedPart = part;
                        part.isOnField = true;
                        part.rootBlock = this;
                        eventData.Use();
                    }
                }
            }
        }
    } 
}
