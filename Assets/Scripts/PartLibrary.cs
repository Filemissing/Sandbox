using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RobotGame
{
    public class PartLibrary : MonoBehaviour, IDropHandler
    {
        public static PartLibrary instance;
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public enum Categories
        {
            None = 0,
            Blocks = 1,
            Weapons = 2,
            Wheels = 4
        }

        Categories displayCategories = Categories.Blocks | Categories.Weapons | Categories.Wheels;
        bool IsCategoryEnabled(Categories categorie)
        {
            if (categorie == Categories.None) return false;

            return (displayCategories & categorie) == categorie;
        }
        void SetCategoryEnabled(Categories categorie, bool enabled)
        {
            if (enabled)
                displayCategories |= categorie;
            else
                displayCategories &= ~categorie;
        }

        [SerializeField] Inventory inventory;
        [SerializeField] BuildObject seatSO;
        List<Part> parts = new List<Part>();
        private void Start()
        {
            inventory.parts.TryGetValue(seatSO, out int value);

            if (value != 1)
                inventory.parts[seatSO] = 1;

            foreach (BuildObject partData in inventory.parts.Keys)
            {
                for (int i = 0; i < inventory.parts[partData]; i++)
                {
                    CreatePart(partData);
                }
            }

            UpdateDisplay();
        }

        int lastPosition = 0; // this points to the end of the row of displaying blocks
        void UpdateDisplay()
        {
            lastPosition = 0; // reset starting point

            for (int i = 0; i < parts.Count; i++)
            {
                Part part = parts[i];
                SetPartPosition(part);
            }
        }
        void SetPartPosition(Part part)
        {
            // universal stuff
            part.transform.SetParent(transform);
            part.gameObject.SetActive(true);
            part.isOnField = false;
            part.rootBlock = null;

            if (part is Block block && IsCategoryEnabled(Categories.Blocks))
            {
                part.transform.position = transform.position + Vector3.right * (lastPosition);
                lastPosition += 2;
                return;
            }
            if (part is Weapon weapon && IsCategoryEnabled(Categories.Weapons))
            {
                part.transform.position = transform.position + Vector3.right * (lastPosition + (part.space.width / 2));
                lastPosition += 1 + part.space.width;
                return;
            }
            if (part is Wheel wheel && IsCategoryEnabled(Categories.Wheels))
            {
                part.transform.position = transform.position + Vector3.right * (lastPosition + (part.space.width / 2));
                lastPosition += 1 + part.space.width;
                return;
            }

            // if display category is disabled
            part.gameObject.SetActive(false);
            part.transform.position = Vector3.zero;
            return;
        }

        public Part CreatePart(BuildObject partData)
        {
            Part part = Instantiate(partData.prefab, transform).GetComponent<Part>();
            parts.Add(part);

            UpdateDisplay();

            if (partData == seatSO)
            {
                BuildField.instance.seat = part as Block;
            }

            return part;
        }
        public void ReturnPart(Part part)
        {
            if (part == null) return;

            // if it was on the field remove it (but only if still marked)
            if (part.isOnField)
            {
                Vector3Int pos = Vector3Int.RoundToInt(part.transform.position);
                BuildField.instance.RemovePart(part, new Vector2Int(pos.x, pos.y));
            }

            parts.Add(part);

            SetPartPosition(part);
        }
        public void RemovePart(Part part)
        {
            parts.Remove(part);
            UpdateDisplay();
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            if (eventData.pointerDrag.TryGetComponent(out Part part))
            {
                ReturnPart(part);
                eventData.Use();
            }
        }
    }
}
