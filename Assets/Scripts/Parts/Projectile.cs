using UnityEngine;

namespace RobotGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float damage = 50f;

        [HideInInspector] public RangedWeapon owner;

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.TryGetComponent<Part>(out Part part);
            if (part != null && part.transform.root != owner.transform.root)
            {
                part.hp -= damage;
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

                Destroy(gameObject);
            }
        }
    }
}
