using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RobotGame
{
    public class BuildField : MonoBehaviour, IDropHandler
    {
        public static BuildField instance;


        [SerializeField] RectInt rect;

        // true is occupied, false is free
        Block[,] backField; // contains building blocks
        (Part, bool)[,] frontField; // contains weapons and wheels, bool isRoot for multi-cell parts

        public Transform robotParent;

        private void Awake()
        {
            instance = this;
            backField = new Block[rect.width, rect.height];
            frontField = new (Part, bool)[rect.width, rect.height];

            // set texture scale and offset
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial.SetTextureScale("_BaseMap", rect.size);
            meshRenderer.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(.5f, .5f));
        }

        public bool TryAddPart(Part part, Vector2Int pos)
        {
            Vector2Int position = pos - rect.position;
            if (part is Block block)
            {
                if(part.space.width > 1 || part.space.height > 1)
                {
                    Debug.LogError("Blocks larger than 1x1 are not supported yet.");
                    return false;
                }

                if (backField[position.x, position.y] == null)
                {
                    backField[position.x, position.y] = block;

                    block.transform.SetParent(robotParent);
                    return true;
                }
                return false;
            }
            else
            {
                if (backField[position.x, position.y] == null) return false; // must be attached to a block

                bool canPlace = true;
                RectInt space = part.space;
                foreach (Vector2Int pos1 in space.allPositionsWithin)
                {
                    Vector2Int adjustedPos = Vector2Int.RoundToInt(position) + pos1;

                    if (adjustedPos.x < 0 || adjustedPos.x >= rect.width || adjustedPos.y < 0 || adjustedPos.y >= rect.height)
                    {
                        canPlace = false;
                        break;
                    }

                    if (frontField[adjustedPos.x, adjustedPos.y].Item1 != null)
                    {
                        canPlace = false;
                        break;
                    }
                }
                if (canPlace)
                {
                    foreach (Vector2Int pos1 in space.allPositionsWithin)
                    {
                        Vector2Int adjustedPos = Vector2Int.RoundToInt(position) + pos1;
                        frontField[adjustedPos.x, adjustedPos.y].Item1 = part;

                        if (backField[adjustedPos.x, adjustedPos.y] != null)
                        {
                            backField[adjustedPos.x, adjustedPos.y].attachedPart = part;
                        }
                    }

                    frontField[position.x, position.y].Item2 = true; // mark root
                    part.rootBlock = backField[position.x, position.y];
                    part.transform.SetParent(robotParent);
                }
                return canPlace;
            }
        }
        public Vector2Int GetRoot(Vector2Int position)
        {
            (Part part, bool isRoot) = frontField[position.x, position.y];
            if(part == null)
            {
                throw new Exception("No part at given position.");
            }

            return GetRoot(part);
        }
        public Vector2Int GetRoot(Part part)
        {
            if(part is Block)
            {
                return Vector2Int.RoundToInt(part.transform.position) - rect.position;
            }
            else
            {
                return Vector2Int.RoundToInt(part.rootBlock.transform.position) - rect.position;
            }
        }
        public void RemovePart(Part part, Vector2Int pos)
        {
            Vector2Int root = GetRoot(part);

            if (part is Block block)
            {
                if (block.attachedPart != null)
                {
                    PartLibrary.instance.ReturnPart(block.attachedPart);
                    block.attachedPart = null;
                }

                backField[root.x, root.y] = null;
                block.isOnField = false;
                return;
            }

            // non block part
            foreach (var cell in part.space.allPositionsWithin)
            {
                Vector2Int p = root + cell;


                frontField[p.x, p.y] = (null, false);


                Block b = backField[p.x, p.y];

                if (b != null)
                    b.attachedPart = null;
            }

            part.isOnField = false;
            part.rootBlock = null;
            part.transform.SetParent(null);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag.TryGetComponent<Part>(out Part part))
            {
                if(TryAddPart(part, Vector2Int.RoundToInt(part.GetWorldPositionOnPlane(eventData.position, part.zOffset))))
                {
                    part.isOnField = true;
                    eventData.Use();
                }
            }
        }
    }
}
