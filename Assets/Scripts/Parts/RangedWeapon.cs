using System.Collections;
using UnityEngine;

namespace RobotGame
{
    public class RangedWeapon : Weapon
    {
        [SerializeField] Projectile projectilePrefab;
        [SerializeField] Transform firePoint;
        [SerializeField] Vector3 projectileSpeed;

        [SerializeField] float reloadTime = 1f;

        public override void Activate()
        {
            base.Activate();
            StartCoroutine(FireLoop());
        }

        IEnumerator FireLoop()
        {
            while (isActive)
            {
                yield return new WaitForSeconds(reloadTime);
                Projectile projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                projectile.owner = this;
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = firePoint.TransformDirection(projectileSpeed);
                }
            }
        }
    }
}
