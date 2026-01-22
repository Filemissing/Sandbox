using System.Collections;
using UnityEngine;

namespace RobotGame
{
    public class Rocket : Propulsion
    {
        [SerializeField] Vector3 acceleration;
        [SerializeField] float duration;
        [SerializeField] float delay;
        Rigidbody rb;

        public override void SetActive(Rigidbody rb)
        {
            this.rb = rb;
            StartCoroutine(ActivateRocket());
        }

        IEnumerator ActivateRocket()
        {
            yield return new WaitForSeconds(delay);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                rb.AddForce(acceleration * Time.deltaTime, ForceMode.Acceleration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
