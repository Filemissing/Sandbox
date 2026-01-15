using System.Collections.Generic;
using UnityEngine;

namespace RobotGame
{
	public class Weapon : Part
	{
		[SerializeField] float dps;
		[SerializeField] BoxCollider hitbox;

		Dictionary<Collider, Part> cache = new();

        private void OnTriggerStay(Collider other)
        {
			Debug.Log("Weapon hit: " + other.name);

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