using UnityEngine;

namespace RobotGame
{
    public class Seat : Block
    {
        public bool isEnemySeat = false;

        Rigidbody rb;

        private void Start()
        {
            rb = transform.root.GetComponent<Rigidbody>();

            if (isEnemySeat)
            {
                foreach (var part in GetComponentsInChildren<Part>())
                {
                    if (part is Propulsion propulsion)
                    {
                        propulsion.SetActive(rb);
                    }
                    if (part is Weapon weapon)
                    {
                        weapon.Activate();
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (isEnemySeat)
            {
                UIManager.instance.ShowWinScreen();
            }
            else
            {
                UIManager.instance.ShowLoseScreen();
            }
        }
    }
}