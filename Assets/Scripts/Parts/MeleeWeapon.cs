using System.Collections.Generic;
using UnityEngine;

namespace RobotGame
{
    public class MeleeWeapon : Weapon
    {
        [SerializeField] float dps;

        Dictionary<Collider, Part> cache = new();

        private void OnTriggerStay(Collider other)
        {
            Part part = cache.ContainsKey(other) ? cache[other] : other.GetComponentInParent<Part>();

            if (!cache.ContainsKey(other))
                cache[other] = part;

            if (part)
            {
                if (part.transform.root != transform.root) // don't damage own parts
                {
                    part.hp -= dps * Time.deltaTime;
                    if (part.hp <= 0)
                    {
                        for (int i = 0; i < part.transform.childCount; i++)
                        {
                            part.transform.GetChild(i).TryGetComponent<Part>(out Part childPart);
                            if (childPart)
                            {
                                childPart.Detatch();
                            }
                        }

                        Destroy(part.gameObject);
                    }
                }
            }
        }
    }
}