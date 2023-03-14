using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIPauseMenu : MonoBehaviour
{
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
    PlayerManager _playerManager;
    List<UIButtonAction> _buttonActions;
    UIButtonAction _currentSelected;
    private void Awake()
    {
        _playerManager = GameManager.Instance.Player.GetComponent<PlayerManager>();
        _buttonActions = new List<UIButtonAction>();
    }

    private void OnEnable()
    {
        InventoryBackground.SetActive(true);
        KeyObjectInventoryBackground.SetActive(false);
        OptionsMenu.SetActive(false);

        if (_buttonActions.Count > 0)
        {
            int count = _buttonActions.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(_buttonActions[i].gameObject);
            }
            _buttonActions.Clear();
        }

        foreach (var inventoryObject in _playerManager.InventoryArray.Where(x => x != null && x.PickableSO != null && !x.PickableSO.IsKeyObject))
        {
            var newButton = Instantiate(buttonActionPrefab, gridEquippablePanel.transform);
            newButton.Initialize(inventoryObject, this, _playerManager.Inventory.EquipmentSlots.Where(x => x != null).ToArray(), _playerManager.Inventory.ActiveObjectSlots.Where(x => x != null).ToArray());
            _buttonActions.Add(newButton);
        }

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
            SelectButton(buttonAction);
        else if (_currentSelected.ActionType == EButtonActionType.InventoryObject && buttonAction.ActionType != EButtonActionType.InventoryObject && !_currentSelected.ObjectInfos.PickableSO.IsKeyObject)
        {
            if (_currentSelected.ObjectInfos.PickableSO.IsConsumable && buttonAction.ActionType == EButtonActionType.ActiveObject)
            {
                TryEquipSlot(buttonAction);
            }
            else if (!_currentSelected.ObjectInfos.PickableSO.IsConsumable && buttonAction.ActionType == EButtonActionType.EquipmentObject)
            {
                TryEquipSlot(buttonAction);
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

    private void TryEquipSlot(UIButtonAction buttonAction)
    {
        if (_playerManager.TryEquip(_currentSelected.ObjectInfos, buttonAction.ActionType, buttonAction.SlotIndex))
        {
            if(buttonAction.ObjectInfos != null)
            {
                var buttonToRestore = _buttonActions.Find(x => x.ObjectInfos.ID == buttonAction.ObjectInfos.ID);
                if (buttonToRestore != null)
                    buttonToRestore.SetQuantity(buttonAction.ObjectInfos.Quantity.ToString());
            }
            buttonAction.OverWrite(_currentSelected.ObjectInfos);
        }
        _currentSelected.SetQuantity("E");
        _currentSelected = null;
    }

    public void RemoveSelectedEquipment()
    {
        if(_currentSelected != null && _currentSelected.ObjectInfos != null && _currentSelected.ObjectInfos.PickableSO != null)
        {
            _playerManager.UnEquip(_currentSelected.ObjectInfos, _currentSelected.ActionType, _currentSelected.SlotIndex);
            var buttonToRestore = _buttonActions.Find(x => x.ObjectInfos.ID == _currentSelected.ObjectInfos.ID);
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
                if(_currentSelected.ObjectInfos.Quantity <= 0)
                {
                    DestroyButton(_currentSelected);
                }
                else
                {
                    DecreaseQuantity(_currentSelected);
                }
            }
        }
    }

    private void DecreaseQuantity(UIButtonAction button)
    {
        button.SetQuantity(button.ObjectInfos.Quantity.ToString());
    }

    private void DestroyButton(UIButtonAction button)
    {
        var buttonToRemove = _buttonActions.Find(x => x.ObjectInfos.ID == button.ObjectInfos.ID);
        _buttonActions.Remove(buttonToRemove);
        Destroy(buttonToRemove.gameObject);

        RemoveSelectedEquipment();
        ClearInfos();
        useButton.gameObject.SetActive(false);
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

    internal void UpdateButton(int slotIndex, int quantityLeft)
    {
        var button = actionButtons[slotIndex];
        if(button != null)
        {
            if (quantityLeft > 0)
                DecreaseQuantity(button);
            else
                DestroyButton(button);
        }
    }
}
