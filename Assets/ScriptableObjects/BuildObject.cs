using UnityEngine;
[CreateAssetMenu(fileName = "BuildObject", menuName = "Scriptable Objects/BuildObject")]
public class BuildObject : ScriptableObject
{
    public ComponentTypes type;
    public Texture texture;
    public int cost;
}