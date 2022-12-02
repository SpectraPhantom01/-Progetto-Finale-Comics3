using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    bool IsPlayer;
    AI aiManager;
    EnemyController enemyController;
    PlayerManager playerManager;

    EDirection lastDirection;
    GameObject _graphics;

    private void Awake()
    {
        spriteRenderer = gameObject.SearchComponent<SpriteRenderer>();
        playerManager = gameObject.SearchComponent<PlayerManager>();
        aiManager = gameObject.SearchComponent<AI>();
        enemyController = (EnemyController)aiManager;
        IsPlayer = playerManager != null;

        _graphics = spriteRenderer.gameObject;
        lastDirection = EDirection.Down;
    }

    private void Update()
    {
        _graphics.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (IsPlayer)
        {
            // player changin sprite
        }
        else
        {
            Vector3 currentVelocity = aiManager.Agent.velocity;

            lastDirection = CalculateDirection(currentVelocity);
        }
    }


    private EDirection CalculateDirection(Vector3 velocity)
    {
        if(velocity.magnitude <= 0.01f)
            return lastDirection;

        
        if(Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)) // horizontal priority
        {
            if(velocity.x > 0)
            {
                return EDirection.Right;
            }
            else
            {
                return EDirection.Left;
            }
        }
        else // vertical priority
        {
            if(velocity.y > 0)
            {
                return EDirection.Up;
            }
            else
            {
                return EDirection.Down;
            }
        }
    }
}
