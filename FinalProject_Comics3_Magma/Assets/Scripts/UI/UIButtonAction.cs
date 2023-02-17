using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonAction : MonoBehaviour
{
    public EButtonActionType ActionType;
    [SerializeField] Image equipImage;
    [SerializeField] UIPauseMenu pauseMenu;
    public int SlotIndex;
    [SerializeField] Sprite defaultSprite;
    [HideInInspector] public Pickable ObjectInfos;
    Button thisButton;
    private void Awake()
    {
        thisButton = GetComponent<Button>();
    }

    public void Initialize(Pickable scriptableObject, UIPauseMenu uIPauseMenu)
    {
        ObjectInfos = scriptableObject;
        pauseMenu = uIPauseMenu;
        SetSprite(scriptableObject.PickableSO.ObjectInventorySprite);
        ActionType = EButtonActionType.InventoryObject;

        thisButton.interactable = true;
    }
    public void OverWrite(Pickable scriptableObject, UIPauseMenu uIPauseMenu)
    {
        ObjectInfos = scriptableObject;
        SetSprite(scriptableObject.PickableSO.ObjectInventorySprite);

        thisButton.interactable = true;
    }

    public void Clear()
    {
        ObjectInfos = null;
        SetSprite(defaultSprite);
    }

    public void SetSprite(Sprite equipmentSprite)
    {
        equipImage.sprite = equipmentSprite;
    }

    public void Action()
    {
        if(pauseMenu != null)
            pauseMenu.SetSelectedObject(ObjectInfos, this);
    }

}

public enum EButtonActionType
{
    InventoryObject,
    EquipmentObject,
    ActiveObject
}
