using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaTriggerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    [SerializeField] bool ignoreGhost;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            if (ignoreGhost && player.PlayerController.ImGhost)
                return;

            onTriggerEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            onTriggerExit.Invoke();
        }
    }

    public void LoadScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}
