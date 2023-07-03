using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIOptions : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] TextMeshProUGUI textPrefab;
    [HideInInspector] public bool IsFirstOpen = true;

    private void OnEnable()
    {
        if (IsFirstOpen)
        {
            IsFirstOpen = false;
            UIManager.Instance.OpenWrittenPanel("Here you can active or deactive the sound, exit the game and read all the logs you've opened until now");
        }
    }

    public void Add(string message)
    {
        var newText = Instantiate(textPrefab, container);
        newText.text = message;
    }
}
