using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [Header("Blocks")]
    [SerializeField] private int woodCount = 0;
    [SerializeField] private int brickCount = 0;
    [SerializeField] private int metalCount = 0;
    [SerializeField] private int chairCount = 1;

    [Header("Wheels")]
    [SerializeField] private int smallWheelCount = 0;
    [SerializeField] private int bigWheelCount = 0;

    [Header("Weapons")]
    [SerializeField] private int chainsawCount = 0;
    [SerializeField] private int dynamiteCount = 0;

    [Header("Money")]
    public int money = 100;

    public void AddMaterial(string componentType)
    {
        FieldInfo field = this.GetType().GetField(componentType.ToString() + "Count", BindingFlags.NonPublic | BindingFlags.Instance);

        if(field == null) 
            throw new System.Exception("No such component in inventory: " + componentType.ToString());
        
        field.SetValue(this, (int)field.GetValue(this) + 1);
    }
    public void AddMoney(int value)
    {
        money += Mathf.Abs(value);
    }
    public void SubtractMoney(int value)
    {
        money = Mathf.Max(0, money - Mathf.Abs(value));
    }
}