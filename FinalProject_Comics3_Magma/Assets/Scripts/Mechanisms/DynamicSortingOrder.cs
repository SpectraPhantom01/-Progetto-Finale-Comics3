using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSortingOrder : MonoBehaviour
{
    MeshRenderer thisSkeletonRenderer;
    int playerDefaultSortingOrder;
    Transform playerController;
    private void Start()
    {
        playerController = GameManager.Instance.Player.transform;
        playerDefaultSortingOrder = GameManager.Instance.Player.PlayerManager.GetDefaultSortingOrder();
        thisSkeletonRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if(gameObject.transform.position.y > playerController.position.y && thisSkeletonRenderer.sortingOrder != playerDefaultSortingOrder - 1)
        {
            thisSkeletonRenderer.sortingOrder = playerDefaultSortingOrder - 1;
        }
        else if(gameObject.transform.position.y < playerController.position.y && thisSkeletonRenderer.sortingOrder != playerDefaultSortingOrder + 1)
        {
            thisSkeletonRenderer.sortingOrder = playerDefaultSortingOrder + 1;
        }
    }
}
