using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaTriggerEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    [SerializeField] bool ignoreGhost;
    [SerializeField] GameObject sfxToSpawnOnTrigger;

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
            if (ignoreGhost && player.PlayerController.ImGhost)
                return;

            onTriggerExit.Invoke();
        }
    }

    public void SpawnSound()
    {
        if (sfxToSpawnOnTrigger != null)
            Instantiate(sfxToSpawnOnTrigger, transform.position, Quaternion.identity);
    }

    public void LoadScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }

    public void OpenMessage(string message)
    {
        UIManager.Instance.OpenWrittenPanel(message);
        Time.timeScale = 0;
    }
}
