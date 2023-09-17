using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "ScriptableObject/Inventory")]
[Serializable]
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

[Serializable]
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

[Serializable]
public class InventoryJSON
{
    public PickableJSON[] InventoryObjects;
    public PickableJSON[] EquipmentSlots;
    public PickableJSON[] ActiveObjectSlots;

    public InventoryJSON(Inventory inventory)
    {
        InventoryObjects = new PickableJSON[inventory.InventoryObjects.Length];
        for (int i = 0; i < inventory.InventoryObjects.Length; i++)
        {
            if (inventory.InventoryObjects[i] != null && inventory.InventoryObjects[i].PickableSO != null)
            {
                InventoryObjects[i] = new PickableJSON();
                InventoryObjects[i].EffectType = inventory.InventoryObjects[i].PickableSO.PickableEffectType;
                InventoryObjects[i].Quantity = inventory.InventoryObjects[i].Quantity;
            }
        }

        ActiveObjectSlots = new PickableJSON[inventory.ActiveObjectSlots.Length];
        for (int i = 0; i < inventory.ActiveObjectSlots.Length; i++)
        {
            if (inventory.ActiveObjectSlots[i] != null && inventory.ActiveObjectSlots[i].PickableSO != null)
            {
                ActiveObjectSlots[i] = new PickableJSON();
                ActiveObjectSlots[i].EffectType = inventory.ActiveObjectSlots[i].PickableSO.PickableEffectType;
                ActiveObjectSlots[i].Quantity = inventory.ActiveObjectSlots[i].Quantity;
            }
        }

        EquipmentSlots = new PickableJSON[inventory.EquipmentSlots.Length];
        for (int i = 0; i < inventory.EquipmentSlots.Length; i++)
        {
            if (inventory.EquipmentSlots[i] != null && inventory.EquipmentSlots[i].PickableSO != null)
            {
                EquipmentSlots[i] = new PickableJSON();
                EquipmentSlots[i].EffectType = inventory.EquipmentSlots[i].PickableSO.PickableEffectType;
                EquipmentSlots[i].Quantity = inventory.EquipmentSlots[i].Quantity;
            }
        }

    }


}