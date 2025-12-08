using UnityEngine;
[CreateAssetMenu(fileName = "BuildObject", menuName = "Scriptable Objects/BuildObject")]
public class BuildObject : ScriptableObject
{
    public Texture texture;
    public GameObject prefab;
    public int cost;
}