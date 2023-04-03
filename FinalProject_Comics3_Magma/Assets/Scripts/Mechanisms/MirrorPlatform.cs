using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlatform : MonoBehaviour
{
    [SerializeField] List<Vector2> platforms;

    private void Awake()
    {
        foreach(Transform child in transform)
        {
            platforms.Add(child.position);
        }

        if(platforms.Count == 0) 
            Destroy(gameObject);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    var player = collision.gameObject.SearchComponent<PlayerController>();

    //    if(player != null)
    //    {
    //        for (int i = 0; i < platforms.Count; i++)
    //        {
    //            player.GhostPositions.Add(platforms[i]);
    //        }
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    var player = collision.gameObject.SearchComponent<PlayerController>();

    //    if (player != null)
    //    {
    //        for (int i = 0; i < platforms.Count; i++)
    //        {
    //            player.GhostPositions.Remove(platforms[i]);
    //        }
    //    }
    //}
}
