using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIPauseMenu : MonoBehaviour
{
    [Header("Openable")]
    [SerializeField] UISaveBool saveBoolOpened;

    [SerializeField] UIButtonAction buttonActionPrefab;
    [SerializeField] GameObject gridEquippablePanel;

    [SerializeField] Image selectedObjectImage;
    [SerializeField] TextMeshProUGUI objectTitle;
    [SerializeField] TextMeshProUGUI objectType;
    [SerializeField] TextMeshProUGUI objectDescription;

    [SerializeField] Button removeButton;
    [SerializeField] Button useButton;

    [SerializeField] GameObject InventoryBackground;
    [SerializeField] GameObject KeyObjectInventoryBackground;
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] Sprite defaultSpriteImage;
    [SerializeField] UIButtonAction[] actionButtons;
    [SerializeField] UIButtonAction[] equipmentButtons;

    [Header("Messages")]
    [SerializeField] List<string> messagesOnFirstOpen;

    PlayerManager _playerManager;
    List<UIButtonAction> _pickableButtonInInventory;
    UIButtonAction _currentSelected;


    private void Awake()
    {
        _playerManager = GameManager.Instance.Player.GetComponent<PlayerManager>();
        _pickableButtonInInventory = new List<UIButtonAction>();
    }

    private void OnEnable()
    {
        InventoryBackground.SetActive(true);
        KeyObjectInventoryBackground.SetActive(false);
        OptionsMenu.SetActive(false);

        var inventoryObjects = _playerManager.InventoryArray.Where(x => x != null && x.PickableSO != null && !x.PickableSO.IsKeyObject).ToArray();

        if (_pickableButtonInInventory.Count != inventoryObjects.Length)
        {
            for (int i = 0; i < inventoryObjects.Length; i++)
            {
                Pickable inventoryObject = inventoryObjects[i];
                var existingButtonAction = _pickableButtonInInventory
                    .FirstOrDefault(ba => ba.ObjectInfos.PickableSO.IsEqual(inventoryObject.PickableSO));

                if (existingButtonAction == null)
                {
                    var newButton = Instantiate(buttonActionPrefab, gridEquippablePanel.transform);
                    newButton.Initialize(inventoryObject, this,
                        _playerManager.Inventory.EquipmentSlots,
                        _playerManager.Inventory.ActiveObjectSlots);
                    _pickableButtonInInventory.Add(newButton);
                }
                else
                {
                    existingButtonAction.OverWrite(inventoryObject);
                    existingButtonAction.CheckAlreadyEquipped(_playerManager.Inventory.EquipmentSlots, _playerManager.Inventory.ActiveObjectSlots);
                }
            }
        }

        for (int i = 0; i < _playerManager.Inventory.EquipmentSlots.Length; i++)
        {
            var equipSlot = _playerManager.Inventory.EquipmentSlots[i];
            if (equipSlot != null && equipmentButtons[i].ObjectInfos != null && equipmentButtons[i].ObjectInfos.PickableSO == null)
            {
                EquipSlotByIndex(i, equipSlot);
            }
        }

        for (int i = 0; i < _playerManager.Inventory.ActiveObjectSlots.Length; i++)
        {
            var activeObject = _playerManager.Inventory.ActiveObjectSlots[i];
            if (activeObject != null && actionButtons[i].ObjectInfos != null
                && (actionButtons[i].ObjectInfos.PickableSO == null || actionButtons[i].ObjectInfos.Quantity != activeObject.Quantity))
            {
                EquipSlotByIndex(i, activeObject);
            }
            else if (actionButtons[i].ObjectInfos != null && actionButtons[i].ObjectInfos.PickableSO != null)
            {
                actionButtons[i].UpdateText();
                
            }
        }

        if (!saveBoolOpened.OpenedOnce)
        {
            saveBoolOpened.OpenedOnce = true;
            Message();
        }
    }

    private void OnDisable()
    {
        _currentSelected = null;
    }


    public void SetSelectedObject(Pickable pickable, UIButtonAction buttonAction)
    {
        if(pickable != null && pickable.PickableSO != null)
        {
            objectTitle.text = pickable.PickableSO.ObjectName;
            objectType.text = pickable.PickableSO.IsKeyObject ? "Key Object" : pickable.PickableSO.IsConsumable ? "Consumable Object" : "Equipment";
            objectDescription.text = pickable.PickableSO.ObjectDescription;
            selectedObjectImage.sprite = pickable.PickableSO.ObjectInventorySprite;
        }

        if (_currentSelected == null || buttonAction.ActionType == EButtonActionType.InventoryObject)
        {
            SelectButton(buttonAction); // se ho selezionato un bottone della lista dell'inventario e attualmente non c'è un altro bottone premuto
        }
        else if (_currentSelected.ActionType == EButtonActionType.InventoryObject && buttonAction.ActionType != EButtonActionType.InventoryObject && !_currentSelected.ObjectInfos.PickableSO.IsKeyObject)
        {
            if (_currentSelected.ObjectInfos.PickableSO.IsConsumable && buttonAction.ActionType == EButtonActionType.ActiveObject)
            {
                TryEquipSlot(buttonAction); // se l'oggetto è active object
            }
            else if (!_currentSelected.ObjectInfos.PickableSO.IsConsumable && buttonAction.ActionType == EButtonActionType.EquipmentObject)
            {
                TryEquipSlot(buttonAction); // se l'oggetto è equipment
            }

            SelectButton(buttonAction); 
        }
        else if (buttonAction.ActionType != EButtonActionType.InventoryObject && buttonAction.ObjectInfos != null && buttonAction.ObjectInfos.PickableSO != null)
        {
            SelectButton(buttonAction);
        }
        else
            _currentSelected = null;
    }

    private void SelectButton(UIButtonAction buttonAction)
    {
        switch (buttonAction.ActionType)
        {
            case EButtonActionType.InventoryObject:
                removeButton.gameObject.SetActive(false);
                useButton.gameObject.SetActive(false);
                break;
            case EButtonActionType.EquipmentObject:
                removeButton.gameObject.SetActive(true);
                useButton.gameObject.SetActive(false);
                break;
            case EButtonActionType.ActiveObject:
                removeButton.gameObject.SetActive(false);
                useButton.gameObject.SetActive(true);
                break;
        }

        _currentSelected = buttonAction;
    }

    public void TryEquipSlot(UIButtonAction buttonAction)
    {
        if (_playerManager.TryEquip(_currentSelected.ObjectInfos, buttonAction.ActionType, buttonAction.SlotIndex))
        {
            OverwriteButton(buttonAction);
            buttonAction.OverWrite(_currentSelected.ObjectInfos);
        }
        _currentSelected.SetQuantity("E");
        _currentSelected = null;
    }

    private void OverwriteButton(UIButtonAction buttonAction)
    {
        if (buttonAction.ObjectInfos != null && buttonAction.ObjectInfos.PickableSO != null)
        {
            var buttonToRestore = _pickableButtonInInventory.FirstOrDefault(x => x.ObjectInfos.PickableSO != null && x.ObjectInfos.PickableSO.ObjectName == buttonAction.ObjectInfos.PickableSO.ObjectName);
            if (buttonToRestore != null)
                buttonToRestore.SetQuantity(buttonAction.ObjectInfos.Quantity.ToString());
        }
    }

    public void EquipSlotByIndex(int uiButtonActionIndex, Pickable pickable)
    {
        if(pickable.PickableSO != null)
        {
            _playerManager.Equip(pickable, uiButtonActionIndex, pickable.PickableSO.IsActiveObject());
            if (pickable.PickableSO.IsActiveObject())
            {
                OverwriteButton(actionButtons[uiButtonActionIndex]);
                actionButtons[uiButtonActionIndex].OverWrite(pickable);
            }
            else
            {
                OverwriteButton(equipmentButtons[uiButtonActionIndex]);
                equipmentButtons[uiButtonActionIndex].OverWrite(pickable);
            }
        }
    }

    public void RemoveSelectedEquipment()
    {
        if(_currentSelected != null && _currentSelected.ObjectInfos != null && _currentSelected.ObjectInfos.PickableSO != null)
        {
            _playerManager.UnEquip(_currentSelected.ObjectInfos, _currentSelected.ActionType, _currentSelected.SlotIndex);
            var buttonToRestore = _pickableButtonInInventory.FirstOrDefault(x => x.ObjectInfos.PickableSO != null && x.ObjectInfos.PickableSO.ObjectName == _currentSelected.ObjectInfos.PickableSO.ObjectName);
            if(buttonToRestore != null)
                buttonToRestore.SetQuantity(_currentSelected.ObjectInfos.Quantity.ToString());
            _currentSelected.Clear();
            _currentSelected = null;
            removeButton.gameObject.SetActive(false);
        }
    }

    public void UseSelectedActionObject()
    {
        if (_currentSelected != null && _currentSelected.ObjectInfos != null && _currentSelected.ObjectInfos.PickableSO != null)
        {
            if(_playerManager.TryUseObject(_currentSelected.ObjectInfos, _currentSelected.SlotIndex))
            {

                DecreaseQuantity(_currentSelected);
            }
        }
    }

    private void DecreaseQuantity(UIButtonAction button)
    {
        button.SetQuantity(button.ObjectInfos.Quantity.ToString());
    }

    public void GoToMenu(int index)
    {
        switch(index)
        {
            case 0:
                InventoryBackground.SetActive(true);
                KeyObjectInventoryBackground.SetActive(false);
                OptionsMenu.SetActive(false);
                break;
            case 1:
                InventoryBackground.SetActive(false);
                KeyObjectInventoryBackground.SetActive(true);
                OptionsMenu.SetActive(false);
                break;
            case 2:
                InventoryBackground.SetActive(false);
                KeyObjectInventoryBackground.SetActive(false);
                OptionsMenu.SetActive(true);
                break;
        }
    }

    public void ClearInfos()
    {
        objectTitle.text = "";
        objectType.text = "";
        objectDescription.text = "";
        selectedObjectImage.sprite = defaultSpriteImage;
    }

    internal void UpdateButton(int slotIndex)
    {
        var button = actionButtons[slotIndex];
        if(button != null)
        {
            DecreaseQuantity(button);
        }
    }

    public void Message()
    {
        UIManager.Instance.OpenWrittenPanel(messagesOnFirstOpen[0], messagesOnFirstOpen);
    }
}
