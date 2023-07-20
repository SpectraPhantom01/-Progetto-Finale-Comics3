using System.Collections.Generic;
using UnityEngine;

public class Main_MirrorPlatform : MonoBehaviour
{
    List<Transform> platforms = new List<Transform>();
    [SerializeField] GameObject helpQMessage;
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
            helpQMessage.SetActive(true);

            player.GhostPositions.Add(transform.position);

            foreach (Transform platform in platforms)
            {
                player.GhostPositions.Add(platform.gameObject.transform.position);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerController>();

        if (player != null)
        {
            helpQMessage.SetActive(false);
            player.GhostPositions.Clear();
        }
    }
}
