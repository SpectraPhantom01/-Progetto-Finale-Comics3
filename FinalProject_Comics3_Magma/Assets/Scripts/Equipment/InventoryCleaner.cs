using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCleaner : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] int inventorySlots;
    [SerializeField] int equipmentSlots;
    [SerializeField] int activeObjectsSlots;
    [SerializeField] LogScriptableObject logScriptableObject;
    [SerializeField] List<UISaveBool> saveBoolList;
    private void Start()
    {
        inventory.ResetValues(inventorySlots, equipmentSlots, activeObjectsSlots);
        logScriptableObject.Clear();
        saveBoolList.ForEach(b => b.OpenedOnce = false);
        Destroy(this);
    }
}
