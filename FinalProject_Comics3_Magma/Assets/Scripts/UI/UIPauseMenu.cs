using System.Collections;
using System.Collections.Generic;
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

        foreach (var inventoryObject in _playerManager.InventoryList)
        {
            var newButton = Instantiate(buttonActionPrefab, gridEquippablePanel.transform);
            newButton.Initialize(inventoryObject, this);
            _buttonActions.Add(newButton);
        }
    }

    public void SetSelectedObject(PickableScriptableObject pickableScriptable, UIButtonAction buttonAction)
    {
        if(pickableScriptable != null)
        {
            objectTitle.text = pickableScriptable.ObjectName;
            objectType.text = pickableScriptable.IsKeyObject ? "Key Object" : pickableScriptable.IsConsumable ? "Consumable Object" : "Equipment";
            objectDescription.text = pickableScriptable.ObjectDescription;
            selectedObjectImage.sprite = pickableScriptable.ObjectInventorySprite;
        }

        if(_currentSelected == null)
            _currentSelected = buttonAction;
        else
        {
            if(_currentSelected.ActionType == EButtonActionType.InventoryObject)
            {
                buttonAction.Initialize(pickableScriptable, this);
            }
        }
    }
}
