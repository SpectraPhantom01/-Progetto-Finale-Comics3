using BehaviorDesigner.Runtime.Tasks;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alchemic : EnemyController
{
    [Header("Skeleton Settings")]
    [SerializeField] SkeletonAnimation upSkeleton;
    [SerializeField] SkeletonAnimation downSkeleton;
    [SerializeField] SkeletonAnimation rightSkeleton;
    [SerializeField] SkeletonAnimation leftSkeleton;
    [SerializeField] string idle;
    [SerializeField] string move;
    [SerializeField] string attackSingle;
    [SerializeField] string attackMultiple;
    SkeletonAnimation _currentSkeleton;

    public bool Attacking;

    private void Update()
    {
        CurrentDirection = Agent.velocity.CalculateDirection(CurrentDirection);

        HandleSkeletonRotation();
        if (!Attacking)
            HandleSkeletonAnimation();
    }

    public void AttackSingle()
    {
        Attacking = true;
        _currentSkeleton.state.SetAnimation(0, attackSingle, true);
    }

    public void AttackMultiple()
    {
        Attacking = true;

        _currentSkeleton.state.SetAnimation(0, attackMultiple, true);
    }

    public override void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        base.Initialize(targetBehaviorVariable, playerTarget);
        BehaviorTree.SetVariableValue("AlchemicController", gameObject);
        _currentSkeleton = downSkeleton;
        _currentSkeleton.state.SetAnimation(0, idle, true);
    }

    public void EndAttack()
    {
        Attacking = false;
    }

    public void HandleSkeletonRotation()
    {
        SkeletonAnimation nextSkeleton = null;
        switch (CurrentDirection)
        {
            case EDirection.Up:
                if (upSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(true);
                nextSkeleton = upSkeleton;

                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(false);
                break;
            case EDirection.Down:
                if (downSkeleton.gameObject.activeSelf) return;

                downSkeleton.gameObject.SetActive(true);
                nextSkeleton = downSkeleton;

                upSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(false);
                break;
            case EDirection.Left:
                if (leftSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(false);
                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(true);

                nextSkeleton = leftSkeleton;
                break;
            case EDirection.Right:
                if (rightSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(false);
                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(true);
                leftSkeleton.gameObject.SetActive(false);

                nextSkeleton = rightSkeleton;
                break;
        }

        nextSkeleton.state.SetAnimation(0, _currentSkeleton.AnimationName, _currentSkeleton.loop);
        _currentSkeleton = nextSkeleton;

    }

    public void HandleSkeletonAnimation()
    {
        if (_currentSkeleton != null)
        {
            if (Agent.velocity.magnitude != 0)
            {
                if (_currentSkeleton.AnimationName != move)
                    _currentSkeleton.state.SetAnimation(0, move, true);
            }
            else
            {
                if (_currentSkeleton.AnimationName != idle)
                    _currentSkeleton.state.SetAnimation(0, idle, true);
            }

        }
    }
}
