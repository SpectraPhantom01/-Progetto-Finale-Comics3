using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    MeshRenderer thisSkeletonRenderer;
    SpriteRenderer spriteRenderer;
    int playerDefaultSortingOrder;
    Transform playerController;
    bool useSprite;
    private void Start()
    {
        playerController = GameManager.Instance.Player.transform;
        playerDefaultSortingOrder = GameManager.Instance.Player.PlayerManager.GetDefaultSortingOrder();
        thisSkeletonRenderer = GetComponent<MeshRenderer>();
        if (thisSkeletonRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            useSprite = spriteRenderer != null;
        }
    }

    private void Update()
    {
        if (!useSprite)
            UseMeshRenderer();
        else
            UseSpriteRenderer();
    }

    private void UseMeshRenderer()
    {
        if (gameObject.transform.position.y > playerController.position.y && thisSkeletonRenderer.sortingOrder != playerDefaultSortingOrder - 1)
        {
            thisSkeletonRenderer.sortingOrder = playerDefaultSortingOrder - 1;
        }
        else if (gameObject.transform.position.y < playerController.position.y && thisSkeletonRenderer.sortingOrder != playerDefaultSortingOrder + 1)
        {
            thisSkeletonRenderer.sortingOrder = playerDefaultSortingOrder + 1;
        }
    }
    private void UseSpriteRenderer()
    {
        if (gameObject.transform.position.y > playerController.position.y && spriteRenderer.sortingOrder != playerDefaultSortingOrder - 1)
        {
            spriteRenderer.sortingOrder = playerDefaultSortingOrder - 1;
        }
        else if (gameObject.transform.position.y < playerController.position.y && spriteRenderer.sortingOrder != playerDefaultSortingOrder + 1)
        {
            spriteRenderer.sortingOrder = playerDefaultSortingOrder + 1;
        }
    }
}