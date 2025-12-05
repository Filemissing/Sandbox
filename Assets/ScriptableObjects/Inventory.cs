using UnityEngine;

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

    public void AddMaterial(ComponentTypes componentType)
    {
        switch (componentType)
        {
            case ComponentTypes.Wood:
                woodCount++;
                break;
            case ComponentTypes.Brick:
                brickCount++;
                break;
            case ComponentTypes.Metal:
                metalCount++;
                break;
            case ComponentTypes.Chair:
                chairCount++;
                break;
            case ComponentTypes.SmallWheel:
                smallWheelCount++;
                break;
            case ComponentTypes.BigWheel:
                bigWheelCount++;
                break;
            case ComponentTypes.Dynamite:
                dynamiteCount++;
                break;
            case ComponentTypes.Chainsaw:
                chainsawCount++;
                break;
        }
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