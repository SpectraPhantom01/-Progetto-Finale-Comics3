using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Spine.Unity;
using Spine;

public class PlayerManager : MonoBehaviour, IAliveEntity
{
    [Header("Hourglass Settings")]
    public float TimeLoseSandInHourglass = 1;
    [SerializeField] float amountLoseSand = 1;
    [Header("Attack Settings")]
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;
    [Header("Inventory Settings")]
    public Inventory Inventory;
    [Header("Skeleton Settings")]
    [SerializeField] SkeletonAnimation upSkeleton;
    [SerializeField] SkeletonAnimation downSkeleton;
    [SerializeField] SkeletonAnimation rightSkeleton;
    [SerializeField] SkeletonAnimation leftSkeleton;
    [SerializeField] SkeletonAnimation upSkeletonRun;
    [SerializeField] SkeletonAnimation downSkeletonRun;
    [SerializeField] string idle;
    [SerializeField] string run;
    [SerializeField] string attack;
    [SerializeField] string dash;
    [SerializeField] string idleSword;
    [SerializeField] string runSword;
    [SerializeField] string dashSword;
    [Header("VFX")]
    [SerializeField] ParticleSystem hourglassVFX;
    public EDirection CurrentDirection = EDirection.Down;
    public bool IsAlive { get ; set ; }
    public string Name => "Etim";
    public Damageable Damageable => _damageable;

    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public PlayerController PlayerController => _playerController;
    private float hourglassTimePassed;
    private Damageable _damageable;
    private PlayerController _playerController;
    SkeletonAnimation _currentSkeleton;
    public SkeletonAnimation CurrentSkeleton => _currentSkeleton;
    TrackEntry _trackEntry;
    [HideInInspector] public Pickable[] InventoryArray => Inventory.InventoryObjects;

    public void Kill(Vector3 respawnPosition)
    {

        Debug.Log("animazione SEI MORTO!");
        StartCoroutine(KillCoroutine(respawnPosition));
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
    }

    private IEnumerator KillCoroutine(Vector3 respawnPosition)
    {
        yield return new WaitForSeconds(1);

        transform.position = respawnPosition;
        Damageable.Heal(50);
    }

    private void Awake()
    {
        IsAlive = true;

        _damageable = gameObject.SearchComponent<Damageable>();

        _playerController = gameObject.SearchComponent<PlayerController>();

        _currentSkeleton = downSkeleton;
        _trackEntry = _currentSkeleton.state.SetAnimation(0, idle, true);
    }

    private void Update()
    {

        HandleHourglass();

        if(_playerController.Rigidbody.velocity.magnitude > 0.01f)
            CurrentDirection = _playerController.Rigidbody.velocity.CalculateDirection();

        HandleSkeletonRotation();
        HandleSkeletonAnimation();
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if(hourglassTimePassed >= Damageable.CurrentHourglass.RealTimeLoseSand)
        {
            hourglassTimePassed = 0;
            _damageable.Damage(amountLoseSand, 0, Vector2.zero, 0);
            var emission = hourglassVFX.emission;
            emission.rateOverTime = 5 + Mathf.Abs(Damageable.CurrentHourglass.HourglassLife - 100);

#if UNITY_EDITOR

            Damageable.CurrentHourglass.Calculate_TimeLossXsecond();
#endif

        }
    }



    public void LockMovement(float time)
    {
        if(_playerController.CanMove)
            StartCoroutine(LockCoroutine(time));
            
    }

    private IEnumerator LockCoroutine(float time)
    {
        _playerController.CanMove = false;
        yield return new WaitForSeconds(time);
        _playerController.CanMove = true;
    }

    public void PickUpObject(PickableScriptableObject newObject, GameObject pickableGameObject)
    {
        if (!InventoryArray.Any(x => x.PickableSO == null)) return;

        for (int i = 0; i < InventoryArray.Length; i++)
        {
            if (InventoryArray[i].PickableSO == null)
            {
                Pickable pickableObject = new()
                {
                    PickableSO = newObject
                };
                InventoryArray[i] = pickableObject;
                Destroy(pickableGameObject);
                return;
            }
        }
    }

    public bool TryUseObject(Pickable pickableObject, int slotIndex)
    {
        if(Inventory.ActiveObjectSlots.Any(x => x == pickableObject))
        {
            var objectToUse = Inventory.ActiveObjectSlots[slotIndex];

            switch (objectToUse.PickableSO.PickableEffectType)
            {
                case EPickableEffectType.HealTime:
                    Damageable.Heal(objectToUse.PickableSO.EffectInPercentage);
                    break;
                case EPickableEffectType.HealHourglass:
                    Damageable.HealHourglass(objectToUse.PickableSO.EffectInPercentage);
                    break;
            }

            RemoveObject(pickableObject);

            return true;
        }

        return false;
    }

    public void RemoveObject(Pickable pickableScriptableObject)
    {
        for (int i = 0; i < InventoryArray.Length; i++)
        {
            if (InventoryArray[i] == pickableScriptableObject)
            {
                InventoryArray[i] = null;
                return;
            }    
        }
    }

    public bool TryEquip(Pickable objectInfos, EButtonActionType actionType, int slotIndex)
    {
        switch (actionType)
        {
            case EButtonActionType.EquipmentObject:
                if (Inventory.EquipmentSlots.Where(x => x != null).Any(x => x.ID == objectInfos.ID))
                    return false;

                Inventory.EquipmentSlots[slotIndex] = objectInfos;
                break;
            case EButtonActionType.ActiveObject:
                if (Inventory.ActiveObjectSlots.Where(x => x != null).Any(x => x.ID == objectInfos.ID))
                    return false;

                Inventory.ActiveObjectSlots[slotIndex] = objectInfos;
                break;
        }
        return true;
    }

    public bool UnEquip(Pickable objectInfos, EButtonActionType actionType, int slotIndex)
    {
        switch (actionType)
        {
            case EButtonActionType.EquipmentObject:
                if (!Inventory.EquipmentSlots.Where(x => x != null).Any(x => x.ID == objectInfos.ID))
                    return false;

                Inventory.EquipmentSlots[slotIndex] = null;
                break;
            case EButtonActionType.ActiveObject:
                if (!Inventory.ActiveObjectSlots.Where(x => x != null).Any(x => x.ID == objectInfos.ID))
                    return false;

                Inventory.ActiveObjectSlots[slotIndex] = null;
                break;
        }
        return true;
    }

    public void HandleSkeletonRotation()
    {
        switch (CurrentDirection)
        {
            case EDirection.Up:
                if (_playerController.Rigidbody.velocity.magnitude != 0)
                {
                    if (upSkeletonRun.gameObject.gameObject.activeSelf) return;
                    upSkeletonRun.gameObject.SetActive(true);
                    upSkeleton.gameObject.gameObject.SetActive(false);

                    _currentSkeleton = upSkeletonRun;
                }
                else
                {
                    if (upSkeleton.gameObject.gameObject.activeSelf) return;
                    upSkeleton.gameObject.gameObject.SetActive(true);
                    upSkeletonRun.gameObject.SetActive(false);

                    _currentSkeleton = upSkeleton;
                }

                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(false);
                downSkeletonRun.gameObject.gameObject.SetActive(false);
                break;
            case EDirection.Down:
                if (_playerController.Rigidbody.velocity.magnitude != 0)
                {
                    if (downSkeletonRun.gameObject.gameObject.activeSelf) return;
                    downSkeletonRun.gameObject.gameObject.SetActive(true);
                    downSkeleton.gameObject.SetActive(false);

                    _currentSkeleton = downSkeletonRun;
                }
                else
                {
                    if (downSkeleton.gameObject.activeSelf) return;
                    downSkeleton.gameObject.SetActive(true);
                    downSkeletonRun.gameObject.SetActive(false);

                    _currentSkeleton = downSkeleton;
                }

                upSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(false);
                upSkeletonRun.gameObject.SetActive(false);
                break;
            case EDirection.Left:
                if (leftSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(false);
                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(false);
                leftSkeleton.gameObject.SetActive(true);
                upSkeletonRun.gameObject.SetActive(false);
                downSkeletonRun.gameObject.SetActive(false);

                _currentSkeleton = leftSkeleton;
                break;
            case EDirection.Right:
                if (rightSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(false);
                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(true);
                leftSkeleton.gameObject.SetActive(false);
                upSkeletonRun.gameObject.SetActive(false);
                downSkeletonRun.gameObject.SetActive(false);

                _currentSkeleton = rightSkeleton;
                break;
        }
    }

    public void HandleSkeletonAnimation()
    {
        if(_currentSkeleton != null)
        {
            if(_playerController.IsMoving)
            {
                if(_trackEntry.Animation.Name != run)
                    _trackEntry = _currentSkeleton.state.SetAnimation(0, run, true);
            }
            else
            {
                if (_trackEntry.Animation.Name != idle)
                    _trackEntry = _currentSkeleton.state.SetAnimation(0, idle, true);
            }
        }
    }

    public GameObject GetGameObject() => gameObject;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        var attackShoot = attackScriptableObjects.Find(x => x.AttackType == EAttackType.Shoot);
        if (attackShoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackShoot.shootAttackRangeOfView);
        }
    }

#endif
}

[Serializable]
public class Inventory
{
    public Pickable[] InventoryObjects;
    public Pickable[] EquipmentSlots;
    public Pickable[] ActiveObjectSlots;
}
[Serializable]
public class Pickable
{
    public PickableScriptableObject PickableSO;
    public Guid ID = Guid.NewGuid();
}