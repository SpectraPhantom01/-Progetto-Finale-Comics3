using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> stepAudioList;
    [SerializeField] List<AudioClip> dashAudioList;
    [SerializeField] List<AudioClip> attackAudioList;

    public CheckPoint _currentCheckPoint;
    public EDirection CurrentDirection = EDirection.Down;
    public Vector3 CurrentVectorDirection => CurrentDirection switch { EDirection.Up => Vector3.up, EDirection.Down => Vector3.down, EDirection.Left => Vector3.left, EDirection.Right => Vector3.right, _ => Vector3.down };
    public bool IsAlive { get; set; }
    public string Name => "Etim";
    public Damageable Damageable => _damageable;

    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public PlayerController PlayerController => _playerController;
    private float hourglassTimePassed;
    private Damageable _damageable;
    private PlayerController _playerController;
    private Coroutine stopHourglassCoroutine;
    SkeletonAnimation _currentSkeleton;
    public SkeletonAnimation CurrentSkeleton => _currentSkeleton;
    [HideInInspector] public Pickable[] InventoryArray => Inventory.InventoryObjects;
    UIPlayArea _uiPlayArea;
    bool stoppedHourglass = false;
    public void Kill()
    {

        Debug.Log("animazione SEI MORTO!");
        StartCoroutine(KillCoroutine());
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
    }

    private IEnumerator KillCoroutine()
    {
        GameManager.Instance.EnablePlayerInputs(false);
        _playerController.Rigidbody.velocity = Vector2.zero;
        _playerController.Direction = Vector2.zero;
        var collider = gameObject.SearchComponent<Collider2D>();
        collider.enabled = false;
        yield return new WaitForSeconds(1);

        Respawn(Vector3.zero);
        Damageable.Heal(50);
        GameManager.Instance.EnablePlayerInputs(true);
        collider.enabled = true;
    }

    private void Awake()
    {
        _playerController = gameObject.SearchComponent<PlayerController>();

        if(!_playerController.ImGhost)
        {
            IsAlive = true;
            _damageable = gameObject.SearchComponent<Damageable>();
        }
    }
    private void Start()
    {
        _currentSkeleton = downSkeleton;
        _currentSkeleton.state.SetAnimation(0, idle, true);


        if (!_playerController.ImGhost)
        {
            _uiPlayArea = UIManager.Instance.UIPlayArea;

            Debug.LogWarning("PERCH� NON FUNZIONA????????????????????????????????????????????");

            upSkeleton.state.Event += State_Event;
            downSkeleton.state.Event += State_Event;
            rightSkeleton.state.Event += State_Event;
            leftSkeleton.state.Event += State_Event;
            upSkeletonRun.state.Event += State_Event;
            downSkeletonRun.state.Event += State_Event;

            upSkeleton.gameObject.SetActive(false);
            rightSkeleton.gameObject.SetActive(false);
            leftSkeleton.gameObject.SetActive(false);
            upSkeletonRun.gameObject.SetActive(false);
            downSkeletonRun.gameObject.SetActive(false);

            for (int i = 1; i < Damageable.HourglassesCount; i++)
            {
                _uiPlayArea.AddNewHourglass();
            }
        }
    }

    private void State_Event(TrackEntry trackEntry, Spine.Event e)
    {
        int rndNum;
        if (e.Data.Name == run)
        {
            rndNum = UnityEngine.Random.Range(0, stepAudioList.Count);
            audioSource.PlayOneShot(stepAudioList[rndNum]);
        }
        else if (e.Data.Name == attack)
        {
            rndNum = UnityEngine.Random.Range(0, attackAudioList.Count);
            audioSource.PlayOneShot(attackAudioList[rndNum]);
        }
        else if(e.Data.Name == dash)
        {
            rndNum = UnityEngine.Random.Range(0, dashAudioList.Count);
            audioSource.PlayOneShot(dashAudioList[rndNum]);
        }
    }

    private void Update()
    {
        if(!_playerController.ImGhost && !stoppedHourglass)
            HandleHourglass();

        if (_playerController.Rigidbody.velocity.magnitude > 0.01f)
            CurrentDirection = _playerController.Rigidbody.velocity.CalculateDirection();

        HandleSkeletonRotation();
        HandleSkeletonAnimation();
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if (hourglassTimePassed >= Damageable.CurrentHourglass.RealTimeLoseSand)
        {
            hourglassTimePassed = 0;
            _damageable.Damage(amountLoseSand);
            var emission = hourglassVFX.emission;
            emission.rateOverTime = 5 + Mathf.Abs(Damageable.CurrentHourglass.HourglassLife - 100);

            if (Damageable.CurrentTimeLife < Damageable.CurrentHourglass.Time * 0.30f)
            {
                if (!_uiPlayArea.IsLowSprite)
                    _uiPlayArea.SetSandLevel(ESandLevel.Low);
            }
            else if (Damageable.CurrentTimeLife < Damageable.CurrentHourglass.Time * 0.60f)
            {
                if (!_uiPlayArea.IsMediumSprite)
                    _uiPlayArea.SetSandLevel(ESandLevel.Medium);
            }
            

#if UNITY_EDITOR

            Damageable.CurrentHourglass.Calculate_TimeLossXsecond();
#endif

        }
    }

    public void StopHourglass(float time)
    {
        if(stopHourglassCoroutine == null)
        {
            stopHourglassCoroutine = StartCoroutine(StopHourglassCoroutine(time));
        }
        else
        {
            StopCoroutine(stopHourglassCoroutine);
            stopHourglassCoroutine = StartCoroutine(StopHourglassCoroutine(time));
        }
    }

    private IEnumerator StopHourglassCoroutine(float time)
    {
        stoppedHourglass = true;
        yield return new WaitForSeconds(time);
        stoppedHourglass = false;
    }

    public void PickUpObject(PickableScriptableObject newObject, PickableObject pickableGameObject)
    {
        if (!InventoryArray.Any(x => x != null && x.PickableSO == null)) return;

        for (int i = 0; i < InventoryArray.Length; i++)
        {
            if (InventoryArray[i] == null || InventoryArray[i].PickableSO == null)
            {
                Pickable pickableObject = new()
                {
                    PickableSO = newObject,
                    Quantity = newObject.QuantityOnPick
                };
                InventoryArray[i] = pickableObject;
                pickableGameObject.Picked();
                return;
            }
        }
    }

    public bool TryUseObject(Pickable pickableObject, int slotIndex, bool forceUpdate = false)
    {
        if (Inventory.ActiveObjectSlots.Any(x => x == pickableObject))
        {
            var objectToUse = Inventory.ActiveObjectSlots[slotIndex];

            PickableScriptableObject.UseActiveObject(objectToUse, Damageable, _playerController.AttackPosition, this);

            objectToUse.Quantity--;
            if (objectToUse.Quantity <= 0)
            {
                RemoveObject(pickableObject);
                _uiPlayArea.ResetActiveObject(slotIndex);
            }
            else
                _uiPlayArea.SetActiveObject(slotIndex, objectToUse.PickableSO.ObjectInventorySprite, objectToUse.Quantity);

            if (forceUpdate)
                UIManager.Instance.PauseMenu.UpdateButton(slotIndex, objectToUse.Quantity);


            return true;
        }

        return false;
    }


    public bool TryUseObject(int inventoryIndex)
    {
        var po = Inventory.ActiveObjectSlots[inventoryIndex];
        if (po == null || po.PickableSO == null)
            return false;

        return TryUseObject(po, inventoryIndex, true);
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
                _uiPlayArea.SetActiveObject(slotIndex, objectInfos.PickableSO.ObjectInventorySprite, objectInfos.Quantity);
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
        SkeletonAnimation nextSkeleton = null;
        switch (CurrentDirection)
        {
            case EDirection.Up:
                if (_playerController.Rigidbody.velocity.magnitude != 0)
                {
                    if (upSkeletonRun.gameObject.gameObject.activeSelf) return;
                    upSkeletonRun.gameObject.SetActive(true);
                    upSkeleton.gameObject.gameObject.SetActive(false);

                    nextSkeleton = upSkeletonRun;
                }
                else
                {
                    if (upSkeleton.gameObject.gameObject.activeSelf) return;
                    upSkeleton.gameObject.gameObject.SetActive(true);
                    upSkeletonRun.gameObject.SetActive(false);

                    nextSkeleton = upSkeleton;
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

                    nextSkeleton = downSkeletonRun;
                }
                else
                {
                    if (downSkeleton.gameObject.activeSelf) return;
                    downSkeleton.gameObject.SetActive(true);
                    downSkeletonRun.gameObject.SetActive(false);

                    nextSkeleton = downSkeleton;
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

                nextSkeleton = leftSkeleton;
                break;
            case EDirection.Right:
                if (rightSkeleton.gameObject.activeSelf) return;

                upSkeleton.gameObject.SetActive(false);
                downSkeleton.gameObject.SetActive(false);
                rightSkeleton.gameObject.SetActive(true);
                leftSkeleton.gameObject.SetActive(false);
                upSkeletonRun.gameObject.SetActive(false);
                downSkeletonRun.gameObject.SetActive(false);

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
            if (_playerController.IsMoving)
            {
                if(_playerController.StateMachine.GetState(EPlayerState.Walking) == _playerController.StateMachine.CurrentState)
                {
                    if (_currentSkeleton.AnimationName != run)
                        _currentSkeleton.state.SetAnimation(0, run, true);
                                                
                }else if (_playerController.IsDashing)
                {
                    if (_currentSkeleton.AnimationName != dashSword)
                        _currentSkeleton.state.SetAnimation(0, dashSword, false);
                }                
            }
            else
            {
                if(_playerController.StateMachine.GetState(EPlayerState.Idle) == _playerController.StateMachine.CurrentState)
                {
                    if (_currentSkeleton.AnimationName != idle)
                        _currentSkeleton.state.SetAnimation(0, idle, true);
                }
                          
            }

            if (_playerController.IsAttacking)
            {
                if (_currentSkeleton.AnimationName != attack)
                    _currentSkeleton.state.SetAnimation(0, attack, false);
            }
        }
    }

    public GameObject GetGameObject() => gameObject;

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        if(!_playerController.ImGhost)
            _currentCheckPoint = checkPoint;
    }

    public void RespawnToCheckpoint()
    {
        if (!_playerController.ImGhost)
            Respawn(_currentCheckPoint.transform.position);
    }

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
    public int Quantity;
    public Guid ID = Guid.NewGuid();
}