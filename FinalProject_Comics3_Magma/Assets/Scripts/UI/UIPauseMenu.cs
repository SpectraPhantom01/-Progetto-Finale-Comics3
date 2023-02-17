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
        if(_buttonActions.Count > 0)
        {
            int count = _buttonActions.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(_buttonActions[i].gameObject);
            }
            _buttonActions.Clear();
        }

        foreach (var inventoryObject in _playerManager.InventoryArray.Where(x => x.PickableSO != null))
        {
            var newButton = Instantiate(buttonActionPrefab, gridEquippablePanel.transform);
            newButton.Initialize(inventoryObject, this);
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
            buttonAction.OverWrite(_currentSelected.ObjectInfos, this);
        }
        _currentSelected = null;
    }

    public void RemoveSelectedEquipment()
    {
        if(_currentSelected != null && _currentSelected.ObjectInfos != null && _currentSelected.ObjectInfos.PickableSO != null)
        {
            _playerManager.UnEquip(_currentSelected.ObjectInfos, _currentSelected.ActionType, _currentSelected.SlotIndex);
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
                var buttonToRemove = _buttonActions.Find(x => x.ObjectInfos.ID == _currentSelected.ObjectInfos.ID);
                _buttonActions.Remove(buttonToRemove);
                Destroy(buttonToRemove.gameObject);

                RemoveSelectedEquipment();
                useButton.gameObject.SetActive(false);
            }
        }
    }
}
