using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIOptions : MonoBehaviour
{
    [Header("Openable")]
    [SerializeField] UISaveBool saveBoolOpened;
    [SerializeField] Transform container;
    [SerializeField] TextMeshProUGUI textPrefab;

    private void OnEnable()
    {
        if (!saveBoolOpened.OpenedOnce)
        {
            saveBoolOpened.OpenedOnce = true;
            UIManager.Instance.OpenWrittenPanel("Here you can active or deactive the sound, exit the game and read all the logs you've opened until now");
        }
    }

    public void Add(string message)
    {
        var newText = Instantiate(textPrefab, container);
        newText.transform.SetAsFirstSibling();
        newText.text = message;
    }
}
