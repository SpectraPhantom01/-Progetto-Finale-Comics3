using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LanguageDipendent : MonoBehaviour, ISubscriber
{
    [SerializeField] string Key;

    TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Publisher.Subscribe(this, typeof(ChangeLanguageMessage));
    }

    private void OnDestroy()
    {
        OnDisableSubscribe();
    }
    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(ChangeLanguageMessage));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if(message is ChangeLanguageMessage)
        {
            textMeshProUGUI.text = LanguageManager.GetValue(Key);
        }
    }

}
