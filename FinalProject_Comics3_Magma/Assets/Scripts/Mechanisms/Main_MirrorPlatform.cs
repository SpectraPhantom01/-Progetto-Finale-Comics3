using System.Collections.Generic;
using UnityEngine;

public class Main_MirrorPlatform : MonoBehaviour
{
    List<Transform> platforms = new List<Transform>();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.SearchComponent<MirrorPlatform>())
                platforms.Add(child);
        }

        if (platforms.Count == 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerController>();

        if (player != null)
        {
            foreach (Transform platform in platforms)
            {
                player.GhostPositions.Add(platform.gameObject.transform.position);
            }

            player.GhostPositions.Add(transform.position);
            // TODO: ASSICURARSI LA POSIZIONE INIZIALE DEL PLAYER IN UN'ALTRA MANIERA E SPOSTARE QUESTA CHIAMATA SOPRA IL FOREACH
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerController>();

        if (player != null)
        {
            //foreach (MirrorPlatform platform in platforms)
            //{
            //    player.GhostPositions.Remove(platform.gameObject.transform.position);
            //}

            player.GhostPositions.Clear();
        }
    }
}
