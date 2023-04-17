using BehaviorDesigner.Runtime.Tasks.Unity.UnityDebug;
using System.Collections;
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
            foreach(Transform platform in platforms) 
            { 
                player.GhostPositions.Add(platform.gameObject.transform.position);  
            }

            player.GhostPositions.Add(transform.position);
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
