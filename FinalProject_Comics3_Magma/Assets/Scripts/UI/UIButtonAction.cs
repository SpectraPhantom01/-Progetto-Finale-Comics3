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

        if (activeObjectsSlot.Any(x => x.ID == ObjectInfos.ID) || equipmentSlots.Any(x => x.ID == ObjectInfos.ID))
            SetQuantity("E");
        else
            SetQuantity(ObjectInfos.Quantity.ToString());

        thisButton.interactable = true;
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
