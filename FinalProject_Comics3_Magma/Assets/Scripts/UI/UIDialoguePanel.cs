using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIDialoguePanel : MonoBehaviour
{
    [SerializeField] List<string> Messages;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] UnityEvent onEnd;
    int index = 0;
    public void Next()
    {
        messageText.text = Messages[index];

        if(index + 1 < Messages.Count)
             index++;
        else
            onEnd.Invoke();
    }
}
