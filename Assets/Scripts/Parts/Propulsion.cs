using UnityEngine;

namespace RobotGame
{
    public abstract class Propulsion : Part
    {
        public abstract void SetActive(Rigidbody rb);
    }
}