using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonAction : MonoBehaviour
{
    public EButtonActionType ActionType;
    [SerializeField] Image equipImage;
    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] UIPauseMenu pauseMenu;
    public int SlotIndex;
    [SerializeField] Sprite defaultSprite;
    [HideInInspector] public Pickable ObjectInfos;
    Button thisButton;
    UIKeyObjectsInventory UIKeyObjectsInventory;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
    }

    public void Initialize(Pickable scriptableObject, UIPauseMenu uIPauseMenu, Pickable[] equipmentSlots, Pickable[] activeObjectsSlot)
    {
        ObjectInfos = scriptableObject;
        pauseMenu = uIPauseMenu;
        SetSprite(scriptableObject.PickableSO.ObjectInventorySprite);
        ActionType = EButtonActionType.InventoryObject;

        CheckAlreadyEquipped(equipmentSlots, activeObjectsSlot);

        thisButton.interactable = true;
    }

    public void CheckAlreadyEquipped(Pickable[] equipmentSlots, Pickable[] activeObjectsSlot)
    {
        if (activeObjectsSlot.Any(x => x != null && x.PickableSO != null && x.PickableSO.ObjectName == ObjectInfos.PickableSO.ObjectName)
           || equipmentSlots.Any(x => x != null && x.PickableSO != null && x.PickableSO.ObjectName == ObjectInfos.PickableSO.ObjectName))
            SetQuantity("E");
        else
            SetQuantity(ObjectInfos.Quantity.ToString());
    }

    public void Initialize(Pickable scriptableObject, UIKeyObjectsInventory uiKeyObjectsInventory)
    {
        ObjectInfos = scriptableObject;
        UIKeyObjectsInventory = uiKeyObjectsInventory;
        SetSprite(scriptableObject.PickableSO.ObjectInventorySprite);
        ActionType = EButtonActionType.InventoryObject;
        SetQuantity(ObjectInfos.Quantity.ToString());

        thisButton.interactable = true;
    }

    public void OverWrite(Pickable scriptableObject)
    {
        ObjectInfos = scriptableObject;
        SetSprite(scriptableObject.PickableSO.ObjectInventorySprite);
        SetQuantity(ObjectInfos.Quantity.ToString());

        if(thisButton != null)
            thisButton.interactable = true;
    }

    public void Clear()
    {
        ObjectInfos = null;
        SetSprite(defaultSprite);
        SetQuantity("0");
    }

    public void SetQuantity(string text)
    {
        quantityText.text = text;
    }

    public void UpdateText()
    {
        quantityText.text = ObjectInfos.Quantity.ToString();
    }

    public void SetSprite(Sprite equipmentSprite)
    {
        equipImage.sprite = equipmentSprite;
    }

    public void Action()
    {
        if (pauseMenu != null)
            pauseMenu.SetSelectedObject(ObjectInfos, this);
        else if (UIKeyObjectsInventory != null)
            UIKeyObjectsInventory.SetSelectedObject(ObjectInfos);

    }

}

public enum EButtonActionType
{
    InventoryObject,
    EquipmentObject,
    ActiveObject
}
