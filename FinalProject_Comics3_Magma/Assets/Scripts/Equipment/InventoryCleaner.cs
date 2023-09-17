using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryCleaner : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] int inventorySlots;
    [SerializeField] int equipmentSlots;
    [SerializeField] int activeObjectsSlots;
    [SerializeField] LogScriptableObject logScriptableObject;
    [SerializeField] List<UISaveBool> saveBoolList;
    [SerializeField] SaveSO saveSO;
    public void Delete()
    {
        inventory.ResetValues(inventorySlots, equipmentSlots, activeObjectsSlots);
        logScriptableObject.Clear();
        saveBoolList.ForEach(b => b.OpenedOnce = false);

        saveSO.SceneName = string.Empty;
        saveSO.LastCheckPointPosition = Vector3.zero;

        try
        {
            if (!File.Exists(Application.persistentDataPath + "\\save.txt"))
            {
                File.Create(Application.persistentDataPath + "\\save.txt");
            }
            else
            {
                File.WriteAllText(Application.persistentDataPath + "\\save.txt", string.Empty);
            }

            if (File.Exists(Application.persistentDataPath + "\\inventory.txt"))
            {
                File.WriteAllText(Application.persistentDataPath + "\\inventory.txt", string.Empty);
            }
            else
            {
                File.Create(Application.persistentDataPath + "\\inventory.txt");
            }
        }
        catch
        {

        }
    }
}
