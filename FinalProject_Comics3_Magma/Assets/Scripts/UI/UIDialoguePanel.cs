using System;
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
    [SerializeField] bool useEvents;
    [SerializeField] List<MessageAndEvent> messagesAndEvents;
    [SerializeField] bool callGameManager = true;
    [SerializeField] float alphaTextSpeed = 0.05f;
    int index = 0;
    Coroutine nextCoroutine;
    int count;
    private void Start()
    {
        count = useEvents ? messagesAndEvents.Count : Messages.Count;
    }
    public void Next()
    {
        if(nextCoroutine == null)
            nextCoroutine = StartCoroutine(NextCoroutine());
        else
        {
            StopCoroutine(nextCoroutine);
            nextCoroutine = StartCoroutine(NextCoroutine());
        }
        
    }

    private IEnumerator NextCoroutine()
    {
        while (true)
        {
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, messageText.color.a - alphaTextSpeed);

            if(messageText.color.a <= 0.1f)
            {
                if (useEvents)
                {
                    messageText.text = messagesAndEvents[index].Message;
                    messagesAndEvents[index].Event?.Invoke();
                }
                else
                    messageText.text = Messages[index];

                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 0);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        while (true)
        {
            messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, messageText.color.a + alphaTextSpeed);

            if (messageText.color.a >= 0.9f)
            {
                messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1);
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        if (index + 1 < count)
            index++;
        else
            onEnd.Invoke();
    }

    private void OnEnable()
    {
        if(callGameManager)
            GameManager.Instance.DialogueMessageActive = true;
    }

    private void OnDisable()
    {
        if(callGameManager)
            GameManager.Instance.DialogueMessageActive = false;
    }
}

[Serializable]
public class MessageAndEvent
{
    public string Message;
    public UnityEvent Event;
}