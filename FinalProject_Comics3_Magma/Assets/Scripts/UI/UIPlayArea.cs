using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayArea : MonoBehaviour, ISubscriber
{
    [SerializeField] Sprite SandLevelMax, SandLevelMedium, SandLevelLow;
    [SerializeField] Image SandLevel;
    [SerializeField] Image SandImagePrefab;
    [SerializeField] GameObject HourglassesContent;
    [Header("Equip Slots")]
    [SerializeField] Image equip1Image;
    [SerializeField] TextMeshProUGUI equip1text;
    [SerializeField] Image equip2Image;
    [SerializeField] TextMeshProUGUI equip2text;
    [SerializeField] Image equip3Image;
    [SerializeField] TextMeshProUGUI equip3text;
    [SerializeField] Image equip4Image;
    [SerializeField] TextMeshProUGUI equip4text;
    [SerializeField] Sprite baseSprite;
    List<Image> SandImages;

    public bool IsMaxSprite;
    public bool IsMediumSprite;
    public bool IsLowSprite;

    private void Start()
    {
        Publisher.Subscribe(this, typeof(AddNewHourglass));
        Publisher.Subscribe(this, typeof(UseNextHourglassMessage));
        SandImages = new List<Image>();

        SetSandLevel(ESandLevel.Max);
    }
    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(AddNewHourglass));
        Publisher.Unsubscribe(this, typeof(UseNextHourglassMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if(message is AddNewHourglass)
        {
            AddNewHourglass();
        }
        else if(message is UseNextHourglassMessage)
        {
            if(SandImages.Count > 0)
            {
                Destroy(SandImages[0].gameObject);
                SandImages.RemoveAt(0);
                SetSandLevel(ESandLevel.Max);
            }
        }
    }

    public void AddNewHourglass()
    {
        var newImage = Instantiate(SandImagePrefab, HourglassesContent.transform);
        SandImages.Add(newImage);
    }

    public void SetSandLevel(ESandLevel sandLevel)
    {
        switch (sandLevel)
        {
            case ESandLevel.Max:
                SandLevel.sprite = SandLevelMax;
                IsMaxSprite = true;
                IsMediumSprite = false;
                IsLowSprite = false;
                break;
            case ESandLevel.Medium:
                SandLevel.sprite = SandLevelMedium;
                IsMaxSprite = false;
                IsMediumSprite = true;
                IsLowSprite = false;
                break;
            case ESandLevel.Low:
                SandLevel.sprite = SandLevelLow;
                IsMaxSprite = false;
                IsMediumSprite = false;
                IsLowSprite = true;
                break;
        }
    }

    public void SetActiveObject(int slotIndex, Sprite sprite, int quantity)
    {
        if(slotIndex == 0)
        {
            equip1Image.sprite = sprite;
            equip1text.text = $"{quantity}";
        }
        else if(slotIndex == 1)
        {

            equip2Image.sprite = sprite;
            equip2text.text = $"{quantity}";
        }
        else if(slotIndex == 2)
        {

            equip3Image.sprite = sprite;
            equip3text.text = $"{quantity}";
        }
        else if(slotIndex == 3)
        {

            equip4Image.sprite = sprite;
            equip4text.text = $"{quantity}";
        }
    }

    internal void ResetActiveObject(int slotIndex)
    {
        if (slotIndex == 0)
        {
            equip1Image.sprite = baseSprite;
            equip1text.text = $"0";
        }
        else if (slotIndex == 1)
        {

            equip2Image.sprite = baseSprite;
            equip2text.text = $"0";
        }
        else if (slotIndex == 2)
        {

            equip3Image.sprite = baseSprite;
            equip3text.text = $"0";
        }
        else if (slotIndex == 3)
        {

            equip4Image.sprite = baseSprite;
            equip4text.text = $"0";
        }
    }
}

public enum ESandLevel
{
    Max,
    Medium,
    Low
}
