using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIKeyObjectsInventory : MonoBehaviour
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
    [HideInInspector] public bool IsFirstOpen = true;

    private void Awake()
    {
        _playerManager = GameManager.Instance.Player.GetComponent<PlayerManager>();
        _buttonActions = new List<UIButtonAction>();
    }

    private void OnEnable()
    {
        if (_buttonActions.Count > 0)
        {
            int count = _buttonActions.Count;
            for (int i = 0; i < count; i++)
            {
                Destroy(_buttonActions[i].gameObject);
            }
            _buttonActions.Clear();
        }

        foreach (var inventoryObject in _playerManager.InventoryArray.Where(x => x != null && x.PickableSO != null && x.PickableSO.IsKeyObject))
        {
            var newButton = Instantiate(buttonActionPrefab, gridEquippablePanel.transform);
            newButton.Initialize(inventoryObject, this);
            _buttonActions.Add(newButton);
        }

        if (IsFirstOpen)
        {
            IsFirstOpen = false;
            UIManager.Instance.OpenWrittenPanel("Here you will find all the Key Objects. These objects will be used by Ethim to access new areas and will disappear once use it.");
        }
    }
    public void SetSelectedObject(Pickable pickable)
    {
        if (pickable != null && pickable.PickableSO != null)
        {
            objectTitle.text = pickable.PickableSO.ObjectName;
            objectType.text = pickable.PickableSO.IsKeyObject ? "Key Object" : pickable.PickableSO.IsConsumable ? "Consumable Object" : "Equipment";
            objectDescription.text = pickable.PickableSO.ObjectDescription;
            selectedObjectImage.sprite = pickable.PickableSO.ObjectInventorySprite;
        }

    }
}
