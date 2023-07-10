using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public struct InventoryStruct
{
    public Pickable[] InventoryObjects;
    public Pickable[] EquipmentSlots;
    public Pickable[] ActiveObjectSlots;

    public InventoryStruct(Pickable[] inventoryObjects, Pickable[] equipmentSlots, Pickable[] activeObjectSlots)
    {
        InventoryObjects = inventoryObjects.ToArray();
        EquipmentSlots = equipmentSlots.ToArray();
        ActiveObjectSlots = activeObjectSlots.ToArray();
    }

}
