using UnityEngine;
[CreateAssetMenu(fileName = "BuildObject", menuName = "Scriptable Objects/BuildObject")]
public class BuildObject : ScriptableObject
{
    public string componentName;
    public Texture texture;
    public int cost;
}