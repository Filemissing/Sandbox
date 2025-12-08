using RobotGame;
using UnityEngine;
public class Shop : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    public void BuyComponent(BuildObject _component)
    {
        if (inventory.money >= _component.cost)
        {
            inventory.SubtractMoney(_component.cost);
            inventory.AddMaterial(_component);
            PartLibrary.instance.CreatePart(_component);
        }
    }
}