using UnityEngine;

public class Part : MonoBehaviour
{
    public float weight;
    public float hp;
    public RectInt space;

    public void Detatch()
    {
        transform.SetParent(null);
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = weight;
    }
}
