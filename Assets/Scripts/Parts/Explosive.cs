using System.Collections;
using UnityEngine;

namespace RobotGame
{
    public class Explosive : Weapon
    {
        [SerializeField] float blastRadius;
        [SerializeField] float blastDamage;
        [SerializeField] float delay;

        public override void Activate()
        {
            base.Activate();
            if (delay >= 0)
            {
                StartCoroutine(DelayedExplode());
            }
        }

        IEnumerator DelayedExplode()
        {
            yield return new WaitForSeconds(delay);
            Detonate();
        }

        private void OnDestroy()
        {
             Detonate(false);
            StopAllCoroutines();
        }

        void Detonate(bool destroySelf = true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);
            foreach (var hitCollider in hitColliders)
            {
                Part part = hitCollider.GetComponent<Part>();
                if (part)
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.ClosestPoint(transform.position));
                    float damage = Mathf.Lerp(blastDamage, 0, distance / blastRadius);
                    part.hp -= damage;
                    if (part.hp <= 0)
                    {
                        for (int i = 0; i < part.transform.childCount; i++)
                        {
                            part.transform.GetChild(i).TryGetComponent<Part>(out Part childPart);
                            if (childPart && childPart != this)
                            {
                                childPart.Detatch();
                            }
                        }
                        Destroy(part.gameObject);
                    }
                }
            }

            if (destroySelf)
                Destroy(gameObject);
        }
    }
}
