using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonAction : MonoBehaviour
{
    public EButtonActionType ActionType;
    Button thisButton;
    Image equipImage;
    PickableScriptableObject objectInfos;
    UIPauseMenu pauseMenu;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
        equipImage = GetComponentInChildren<Image>();
    }

    public void Initialize(PickableScriptableObject scriptableObject, UIPauseMenu uIPauseMenu)
    {
        objectInfos = scriptableObject;
        pauseMenu = uIPauseMenu;
        SetSprite(scriptableObject.ObjectInventorySprite);
        ActionType = EButtonActionType.InventoryObject;
    }

    public void SetSprite(Sprite equipmentSprite)
    {
        equipImage.sprite = equipmentSprite;
    }

    public void Action()
    {
        if(pauseMenu != null)
            pauseMenu.SetSelectedObject(objectInfos, this);
    }

    private void OnEnable()
    {
        thisButton.interactable = objectInfos != null;
    }
}

public enum EButtonActionType
{
    InventoryObject,
    EquipmentObject,
    ActiveObject
}
