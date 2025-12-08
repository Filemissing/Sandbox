using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory")]
public class Inventory : ScriptableObject
{
    [Header("Parts")]
    public Dictionary<BuildObject, int> parts = new();

    [Header("Money")]
    public int money = 100;

    public void AddMaterial(BuildObject part)
    {
        parts.TryGetValue(part, out int count);
        parts[part] = count + 1;
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