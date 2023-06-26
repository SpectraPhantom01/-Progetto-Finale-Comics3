using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "ScriptableObject/Inventory")]
public class Inventory : ScriptableObject
{
    public Pickable[] InventoryObjects;
    public Pickable[] EquipmentSlots;
    public Pickable[] ActiveObjectSlots;

    public void ResetValues(int inventoryQuantity, int equipmentQuantity, int activeObjectsQuantity)
    {
        InventoryObjects = new Pickable[inventoryQuantity];
        EquipmentSlots = new Pickable[equipmentQuantity];
        ActiveObjectSlots = new Pickable[activeObjectsQuantity];
    }
}
