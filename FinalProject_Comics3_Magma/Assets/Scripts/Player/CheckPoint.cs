using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            player.SetCheckPoint(this);
        }
    }
}
