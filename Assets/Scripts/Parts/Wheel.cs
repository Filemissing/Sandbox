using UnityEngine;

namespace RobotGame
{
	public class Wheel : Part
	{
		WheelCollider wheelCollider;
        MeshRenderer wheelModel;
        private void Awake()
        {
            wheelCollider = GetComponentInChildren<WheelCollider>();
            wheelModel = GetComponentInChildren<MeshRenderer>();
        }

        public float speed;

        private void Update()
        {
            if (wheelCollider != null)
            {
                wheelCollider.motorTorque = speed;
                wheelModel.transform.position = wheelCollider.transform.position;
                wheelModel.transform.rotation = wheelCollider.transform.rotation;
            }
        }
    } 
}
