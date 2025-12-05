using UnityEngine;
public enum ComponentTypes { Wood, Brick, Metal, Chair, BigWheel, SmallWheel, Dynamite, Chainsaw }
public class Shop : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private void Start()
    {
        UpdateInventory();
    }
    public void BuyComponent(BuildObject _component)
    {
        if (inventory.money >= _component.cost)
        {
            inventory.SubtractMoney(_component.cost);
            inventory.AddMaterial(_component.type);
            UpdateInventory();
        }
    }
    private void UpdateInventory()
    {

    }
}