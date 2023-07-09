using Cinemachine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : EnemyController
{
    [Header("Skeleton Settings")]
    [SerializeField] SkeletonAnimation upSkeleton;
    [SerializeField] SkeletonAnimation downSkeleton;
    [SerializeField] SkeletonAnimation rightSkeleton;
    [SerializeField] SkeletonAnimation leftSkeleton;
    [SerializeField] string idle;
    [SerializeField] string move;
    [SerializeField] string bombAttack;
    [SerializeField] string shootAttack;
    [SerializeField] string speak;
    [SerializeField] string growInAir;
    SkeletonAnimation _currentSkeleton;
    [SerializeField] float bombAttackAnimationTime;
    [SerializeField] float shootAttackAnimationTime;
    [Header("Bullets")]
    [SerializeField] float delayAttackShoot;
    [SerializeField] GameObject bomb;
    [SerializeField] int minBombs = 2;
    [SerializeField] int maxBombs = 6;
    [Header("Refs")]
    [SerializeField] GameObject lifeSliderPanel;
    [SerializeField] Slider lifeSlider;
    public SkeletonAnimation CurrentSkeleton => _currentSkeleton;
    public bool Attacking;
    GameObject targetEnemy;
    float initialtotalLife;
    AudioSource _explosionAudioSource;
    CinemachineImpulseSource _impulseSource;
    public override void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        base.Initialize(targetBehaviorVariable, playerTarget);

        BehaviorTree.SetVariableValue("BossController", gameObject);
        _currentSkeleton = downSkeleton;
        _currentSkeleton.state.SetAnimation(0, idle, true);
        targetEnemy = GameManager.Instance.Player.gameObject;

        Damager.EquipAttack(EAttackType.Shoot);

        initialtotalLife = Damageable.GetTotalLifeTime();

        Damageable.onGetDamage += () =>
        {
            var percent = Damageable.GetRelativeTotalLifeTime() / initialtotalLife;
            lifeSlider.value = percent;
        };

        onKillEnemy += () => lifeSliderPanel.SetActive(false);

        _explosionAudioSource = GetComponent<AudioSource>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    public void AttackShoot()
    {
        DisableBehavior();
        
        Attacking = true;

        _currentSkeleton.state.SetAnimation(0, shootAttack, false);

        Invoke(nameof(DelayAttack), delayAttackShoot);

        StartCoroutine(WaitAnimation(shootAttackAnimationTime));
    }

    private void DelayAttack()
    {
        Damager.Attack();
    }

    public void AttackBomb()
    {
        DisableBehavior();

        var center = targetEnemy.transform.position;
        var ray = center + Vector3.right * 10;
        var distance = Mathf.Sqrt(Mathf.Pow(ray.x - center.x, 2) + Mathf.Pow(ray.y - center.y, 2));

        List<Vector3> positions = new();

        var rayCircleInside = (distance / 2) / Mathf.Sqrt(2);
        int count = UnityEngine.Random.Range(minBombs, maxBombs);
        for (int i = 0; i < count; i++)
        {
            var angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
            positions.Add(new Vector3(rayCircleInside * Mathf.Cos(angle) , rayCircleInside * Mathf.Sin(angle), 0));
        }

        var bombOnPlayer = Instantiate(bomb, targetEnemy.transform.position, Quaternion.identity);
        bombOnPlayer.GetComponent<BombVFX>().onExplosion += PlayBombSound;
        bombOnPlayer.GetComponent<BombVFX>().onExplosion += ShakeCamera;
        Destroy(bombOnPlayer, 7.5f);

        foreach (var pos in positions)
        {
            Destroy(Instantiate(bomb, targetEnemy.transform.position + pos, Quaternion.identity), 7.5f);
        }

        Attacking = true;

        _currentSkeleton.state.SetAnimation(0, bombAttack, false);

        StartCoroutine(WaitAnimation(bombAttackAnimationTime));
    }

    public void PlayBombSound()
    {
        _explosionAudioSource.Play();
    }

    public void ShakeCamera()
    {
        _impulseSource.GenerateImpulse();
    }

    private IEnumerator WaitAnimation(float time)
    {
        yield return new WaitForSeconds(time);
        EnableBehavior();
        Attacking = false;
    }

    private void Update()
    {

        CurrentDirection = Agent.velocity.CalculateDirection(CurrentDirection);

        HandleSkeletonRotation();
        if(!Attacking)
            HandleSkeletonAnimation();
    }

    public void EnableBehavior()
    {
        Agent.enabled = true;
        BehaviorTree.enabled = true;
    }

    public void DisableBehavior()
    {
        Agent.enabled = false;
        BehaviorTree.enabled = false;
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

    public void HandleSkeletonAnimation() //Da sistemare
    {
        if (_currentSkeleton != null)
        {
            if(Agent.velocity.magnitude != 0)
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
