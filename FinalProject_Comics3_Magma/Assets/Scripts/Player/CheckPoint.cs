using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool SaverLevel;
    [SerializeField] List<GameObject> roomsToOpen;
    [SerializeField] List<GameObject> roomsToClose;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            player.SetCheckPoint(this);
            if (SaverLevel)
                GameManager.Instance.SetCheckPoint(this);
        }
    }

    public void LoadRooms()
    {
        foreach (var item in roomsToOpen)
        {
            item.SetActive(true);
        }

        foreach (var item in roomsToClose)
        {
            item.SetActive(false);
        }
    }
}
