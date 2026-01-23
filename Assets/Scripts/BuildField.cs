using System;
using System.Collections.Generic;
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

        public Block seat;
        public Transform robotParent;

        [SerializeField] CanvasGroup seatWarning;

        Vector2Int[] orthogonalDirections = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

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

        public Vector2Int GetRoot(Vector2Int position)
        {
            (Part part, bool isRoot) = frontField[position.x, position.y];
            if (part == null)
            {
                throw new Exception("No part at given position.");
            }

            return GetRoot(part);
        }
        public Vector2Int GetRoot(Part part)
        {
            if (part is Block)
            {
                return Vector2Int.RoundToInt(part.transform.position) - rect.position;
            }
            else
            {
                return Vector2Int.RoundToInt(part.rootBlock.transform.position) - rect.position;
            }
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

        public void Assemble()
        {
            if (!seat.isOnField)
            {
                seatWarning.alpha = 1;
                seatWarning.interactable = true;
                seatWarning.blocksRaycasts = true;
                return;
            }

            GameObject assembly = new GameObject("Assembly");
            GameObject rotationCorrector = new GameObject("RotationObject");

            rotationCorrector.transform.SetParent(assembly.transform);
            seat.transform.SetParent(rotationCorrector.transform);

            Rigidbody rb = assembly.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            // BFS weld
            Queue<Block> weldQueue = new Queue<Block>();
            HashSet<Block> visited = new HashSet<Block>();
            HashSet<Wheel> wheels = new HashSet<Wheel>(); // store all wheels for suspension calculation later

            weldQueue.Enqueue(seat);

            while (weldQueue.Count > 0)
            {
                Block current = weldQueue.Dequeue();

                Part attachedPart = current.attachedPart;
                if (attachedPart != null && attachedPart.rootBlock == current)
                {
                    attachedPart.transform.SetParent(current.transform);
                    if (attachedPart is Propulsion propulsion)
                    {
                        propulsion.SetActive(rb);

                        if (propulsion is Wheel wheel)
                        {
                            wheels.Add(wheel);

                            // clone wheel to other side
                            Wheel wheelClone = Instantiate(wheel, current.transform);
                            wheelClone.transform.localPosition = new Vector3(wheel.transform.localPosition.x, wheel.transform.localPosition.y, -wheel.transform.localPosition.z);
                            wheelClone.SetActive(rb);
                            wheels.Add(wheelClone);
                        }
                    }
                    if (attachedPart is Weapon weapon)
                    {
                        weapon.Activate();
                    }
                }

                foreach (var dir in orthogonalDirections)
                {
                    Vector2Int neighborPos = GetRoot(current) + dir;

                    if (neighborPos.x < 0 || neighborPos.x >= rect.width || neighborPos.y < 0|| neighborPos.y >= rect.height)
                        continue;

                    Block neighborBlock = backField[neighborPos.x, neighborPos.y];
                    if (neighborBlock != null)
                    {
                        if (!visited.Contains(neighborBlock))
                        {
                            neighborBlock.transform.SetParent(current.transform);
                            weldQueue.Enqueue(neighborBlock);
                        }
                    }
                }

                visited.Add(current);
            }

            rotationCorrector.transform.rotation = Quaternion.Euler(0, -90, 0); // adjust rotation to match wheel colliders

            // calculate center of mass
            float totalWeight = 0;
            foreach (Block block in visited)
            {
                totalWeight += block.weight;
                if (block.attachedPart != null && block.attachedPart.rootBlock == block)
                    totalWeight += block.attachedPart.weight;
            }

            Vector3 centerOfMass = Vector3.zero;
            foreach (var block in visited)
            {
                Vector3 relativePos = seat.transform.InverseTransformPoint(block.transform.position);

                float mass = block.weight;

                if (block.attachedPart != null && block.attachedPart.rootBlock == block)
                    mass += block.attachedPart.weight;

                centerOfMass += relativePos * mass;
            }

            // TODO: hack to get total mass to 1800kg needed for the suspension. Better: change suspesion parameters based on mass
            centerOfMass /= totalWeight;
            float factor = 1800 / totalWeight;

            foreach (Block block in visited)
            {
                block.weight *= factor;
            }

            rb.mass = 1800; //  totalWeight;

            // move seat so assembly center of mass is at origin
            seat.transform.localPosition = -centerOfMass;
            rb.centerOfMass = Vector3.zero; // correct since we moved the root to the Center of Mass


            assembly.transform.Translate(0, 2, -10); // move assembly to start position (hopefully)

            SetTagRecursive(assembly, "Player");

            DontDestroyOnLoad(assembly);
            UnityEngine.SceneManagement.SceneManager.LoadScene("Battle Scene");
        }

        void SetTagRecursive(GameObject root, string tag)
        {
            root.tag = tag;
            foreach (Transform child in root.transform)
            {
                SetTagRecursive(child.gameObject, tag);
            }
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
