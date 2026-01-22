using UnityEngine;

namespace RobotGame
{
	public class Wheel : Propulsion
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
            if (wheelCollider != null && wheelCollider.enabled)
            {
                wheelCollider.motorTorque = speed;
                wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion quat);
                Quaternion rotation = Quaternion.Euler(0, 90, quat.eulerAngles.x);
                wheelModel.transform.SetPositionAndRotation(pos, rotation);
            }
        }

        public override void SetActive(Rigidbody rb)
        {
            GetComponent<BoxCollider>().enabled = false;
            wheelCollider.enabled = true;
        }
    } 
}
