using System.IO;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;

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
    [SerializeField] SpriteRenderer slashEffectRenderer;
    [Header("VFX")]
    [SerializeField] ParticleSystem hourglassVFX;
    [SerializeField] GameObject deathVFX;

    private CheckPoint _currentCheckPoint;
    public CheckPoint CheckPoint => _currentCheckPoint;
    public EDirection CurrentDirection = EDirection.Down;
    public Vector3 CurrentVectorDirection => CurrentDirection switch { EDirection.Up => Vector3.up, EDirection.Down => Vector3.down, EDirection.Left => Vector3.left, EDirection.Right => Vector3.right, _ => Vector3.down };
    public bool IsAlive { get; set; }
    public string Name => "Ethim";
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
    bool isTeleporting = false;
    DamagedVolumeHandler _damagedVolumeHandler;
    private InventoryStruct photographInventory;

    private void Awake()
    {
        _playerController = gameObject.SearchComponent<PlayerController>();

        if(!_playerController.ImGhost)
        {
            IsAlive = true;
            _damageable = gameObject.SearchComponent<Damageable>();
            _damagedVolumeHandler = GetComponentInChildren<DamagedVolumeHandler>(true);
        }
    }
    private void Start()
    {
        _currentSkeleton = downSkeleton;
        _currentSkeleton.state.SetAnimation(0, idle, true);


        if (!_playerController.ImGhost)
        {
            _uiPlayArea = UIManager.Instance.UIPlayArea;

            LoadInventory();
            if(photographInventory.InventoryObjects == null)
                photographInventory = new InventoryStruct(InventoryArray, Inventory.EquipmentSlots, Inventory.ActiveObjectSlots);


            UpdateInventoryButtons();

            for (int i = 1; i < Damageable.HourglassesCount; i++)
            {
                AddNewHourglass();
            }

            Damageable.onGetDamage += _damagedVolumeHandler.FadeUp;
        }
    }

    private void UpdateInventoryButtons()
    {
        foreach (var item in InventoryArray.Where(x => x.PickableSO != null))
        {
            var equip = Inventory.EquipmentSlots.FirstOrDefault(e => e.PickableSO != null && e.PickableSO.IsEqual(item.PickableSO));
            if (equip != null)
                equip.Quantity = item.Quantity;

            
            for (int i = 0; i < Inventory.ActiveObjectSlots.Length; i++)
            {
                var active = Inventory.ActiveObjectSlots[i];
                if (active.PickableSO != null && active.PickableSO.IsEqual(item.PickableSO))
                {
                    active.Quantity = item.Quantity;
                    _uiPlayArea.SetActiveObject(i, active.PickableSO.ObjectInventorySprite, active.Quantity);
                }
            }
        }
    }

    public void AddNewHourglass()
    {
        _uiPlayArea.AddNewHourglass();
    }

    private void Update()
    {
        if(!_playerController.ImGhost && !stoppedHourglass && IsAlive)
            HandleHourglass();

        if (_playerController.Rigidbody.velocity.magnitude > 0.01f && PlayerController.CanMove)
            CurrentDirection = _playerController.Rigidbody.velocity.CalculateDirection();

        if (!isTeleporting && IsAlive)
        {
            HandleSkeletonRotation();
            HandleSkeletonAnimation();
        }
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if (hourglassTimePassed >= Damageable.CurrentHourglass.RealTimeLoseSand)
        {
            hourglassTimePassed = 0;
            _damageable.Damage(amountLoseSand);

            if (!IsAlive)
                return;

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
        hourglassVFX.Stop();
        yield return new WaitForSeconds(time);
        stoppedHourglass = false;
        hourglassVFX.Play();
    }

    public void PickUpObject(PickableScriptableObject newObject, PickableObject pickableGameObject)
    {
        if (InventoryArray.Where(p => p != null && p.PickableSO != null).Count() == InventoryArray.Length) return;
        bool found = false;
        var addedPickable = InventoryArray.Where(p => p != null && p.PickableSO != null).ToArray();
        if(addedPickable.Length > 0)
        {
            for (int i = 0; i < addedPickable.Length; i++)
            {
                if(addedPickable[i].PickableSO.IsEqual(newObject))
                {
                    addedPickable[i].Quantity += newObject.QuantityOnPick;
                    found = true;
                    break;
                }
            }
        }

        if(!found)
        {
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
                    break;
                }
            }
        }

        pickableGameObject.Picked();
        UpdateInventoryButtons();
    }

    public bool TryUseObject(Pickable pickableObject, int slotIndex, bool forceUpdate = false)
    {
        if (Inventory.ActiveObjectSlots.Any(x => x == pickableObject))
        {
            var objectToUse = Inventory.ActiveObjectSlots[slotIndex];
            if (objectToUse.Quantity <= 0)
                return false;

            var objectInInventory = Inventory.InventoryObjects.FirstOrDefault(x => x.PickableSO != null && x.PickableSO.IsEqual(objectToUse.PickableSO));

            PickableScriptableObject.UseActiveObject(objectToUse, Damageable, _playerController.AttackPosition, this);

            objectToUse.Quantity--;
            objectInInventory.Quantity = objectToUse.Quantity;

            _uiPlayArea.SetActiveObject(slotIndex, objectToUse.PickableSO.ObjectInventorySprite, objectToUse.Quantity);

            if (forceUpdate)
                UIManager.Instance.PauseMenu.UpdateButton(slotIndex);


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

    public bool TryEquip(Pickable objectInfos, EButtonActionType actionType, int slotIndex)
    {
        switch (actionType)
        {
            case EButtonActionType.EquipmentObject:
                if (Inventory.EquipmentSlots.Where(x => x != null && x.PickableSO != null).Any(x => x.PickableSO.ObjectName == objectInfos.PickableSO.ObjectName))
                    return false;

                Equip(objectInfos, slotIndex);
                break;
            case EButtonActionType.ActiveObject:
                if (Inventory.ActiveObjectSlots.Where(x => x != null && x.PickableSO != null).Any(x => x.PickableSO.ObjectName == objectInfos.PickableSO.ObjectName))
                    return false;

                Equip(objectInfos, slotIndex, true);
                break;
        }
        return true;
    }

    public void Equip(Pickable objectInfos, int slotIndex, bool activeObject = false)
    {

        if(activeObject)
        {
            Inventory.ActiveObjectSlots[slotIndex] = objectInfos;
            _uiPlayArea.SetActiveObject(slotIndex, objectInfos.PickableSO.ObjectInventorySprite, objectInfos.Quantity);
        }
        else
            Inventory.EquipmentSlots[slotIndex] = objectInfos;
    }


    public bool UnEquip(Pickable objectInfos, EButtonActionType actionType, int slotIndex)
    {
        switch (actionType)
        {
            case EButtonActionType.EquipmentObject:
                if (!Inventory.EquipmentSlots.Where(x => x != null && x.PickableSO != null).Any(x => x.PickableSO.ObjectName == objectInfos.PickableSO.ObjectName))
                    return false;

                Inventory.EquipmentSlots[slotIndex] = null;
                break;
            case EButtonActionType.ActiveObject:
                if (!Inventory.ActiveObjectSlots.Where(x => x != null && x.PickableSO != null).Any(x => x.PickableSO.ObjectName == objectInfos.PickableSO.ObjectName))
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

                slashEffectRenderer.flipY = false;
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

                slashEffectRenderer.flipY = false;
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

                slashEffectRenderer.flipY = true;
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

                slashEffectRenderer.flipY = false;
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


    public bool HasObjectInInventory(EPickableEffectType effectType)
    {
        var pick = InventoryArray.Where(pickable => pickable != null && pickable.PickableSO != null).FirstOrDefault(pickable => pickable.PickableSO.PickableEffectType == effectType);
    
        return pick != null && pick.Quantity > 0;
    }

    public void Teleport(Vector3 position, float timeDelayTeleport, GameObject vfxOnTeleportPrefab, UnityEvent onTeleportHalfEvent, UnityEvent onTeleportEndEvent)
    {
        StartCoroutine(TeleportCoroutine(position, timeDelayTeleport, vfxOnTeleportPrefab, onTeleportHalfEvent, onTeleportEndEvent));
    }

    private IEnumerator TeleportCoroutine(Vector3 position, float timeDelayTeleport, GameObject vfxOnTeleportPrefab, UnityEvent onTeleportHalfEvent, UnityEvent onTeleportEndEvent)
    {
        isTeleporting = true;
        _currentSkeleton.gameObject.SetActive(false);
        GameManager.Instance.EnablePlayerInputs(false);

        var time = timeDelayTeleport > 0 ? timeDelayTeleport / 2 : 0;

        yield return new WaitForSeconds(time);

        transform.position = position;

        onTeleportHalfEvent?.Invoke();

        yield return new WaitForSeconds(time);

        onTeleportEndEvent?.Invoke();

        isTeleporting = false;
        _currentSkeleton.gameObject.SetActive(true);
        Instantiate(vfxOnTeleportPrefab, position, Quaternion.identity);
        GameManager.Instance.EnablePlayerInputs(true);
    }

    //private void OnDestroy()
    //{
    //    if (PlayerController.ImGhost) return;

    //    int count = Inventory.ActiveObjectSlots.Length;
    //    Inventory.ActiveObjectSlots = new Pickable[count];

    //    count = Inventory.EquipmentSlots.Length;
    //    Inventory.EquipmentSlots = new Pickable[count];
    //}

    public int GetDefaultSortingOrder()
    {
        if(_currentSkeleton == null)
            return downSkeleton.GetComponent<MeshRenderer>().sortingOrder;
        
        return _currentSkeleton.GetComponent<MeshRenderer>().sortingOrder;
    }

    public void Kill()
    {
        IsAlive = false;
        GameManager.Instance.Save();
        GameManager.Instance.EnablePlayerInputs(false, true);
        PlayerController.Rigidbody.isKinematic = true;
        var colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
        Damageable.enabled = false;
        PlayerController.Damager.enabled = false;
        RestoreFromPhotographInventory();

        GameManager.Instance.EnableCameraGameOver(true);
        deathVFX.SetActive(true);
    }

    public void SaveInventory()
    {
        if (PlayerController.ImGhost) return;
        InventoryToPhotography();

        try
        {

            if (!File.Exists(Application.persistentDataPath + "\\inventory.txt"))
            {
                File.Create(Application.persistentDataPath + "\\inventory.txt");
            }
            else
            {
                File.WriteAllText(Application.persistentDataPath + "\\inventory.txt", JsonConvert.SerializeObject(new InventoryJSON(Inventory)));
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void InventoryToPhotography()
    {
        photographInventory.ActiveObjectSlots = Inventory.ActiveObjectSlots.ToArray();
        photographInventory.EquipmentSlots = Inventory.EquipmentSlots.ToArray();
        photographInventory.InventoryObjects = Inventory.InventoryObjects.ToArray();
    }

    public void RestoreFromPhotographInventory()
    {
        if (PlayerController.ImGhost) return;

        Inventory.ActiveObjectSlots = photographInventory.ActiveObjectSlots.ToArray();
        Inventory.EquipmentSlots = photographInventory.EquipmentSlots.ToArray();
        Inventory.InventoryObjects = photographInventory.InventoryObjects.ToArray();
    }

    private void LoadInventory()
    {
        try
        {
            if (File.Exists(Application.persistentDataPath + "\\inventory.txt"))
            {
                var inventory = File.ReadAllText(Application.persistentDataPath + "\\inventory.txt");

                if (string.IsNullOrEmpty(inventory))
                    return;

                var inventoryJson = JsonUtility.FromJson<InventoryJSON>(inventory);

                for (int i = 0; i < inventoryJson.InventoryObjects.Length; i++)
                {
                    if (inventoryJson.InventoryObjects[i] != null)
                    {
                        Inventory.InventoryObjects[i].PickableSO = LevelManager.Instance.GetPickable(inventoryJson.InventoryObjects[i].EffectType);
                        Inventory.InventoryObjects[i].Quantity = inventoryJson.InventoryObjects[i].Quantity;
                    }
                }

                for (int i = 0; i < inventoryJson.ActiveObjectSlots.Length; i++)
                {
                    if (inventoryJson.ActiveObjectSlots[i] != null)
                    {
                        Inventory.ActiveObjectSlots[i].PickableSO = LevelManager.Instance.GetPickable(inventoryJson.ActiveObjectSlots[i].EffectType);
                        Inventory.ActiveObjectSlots[i].Quantity = inventoryJson.ActiveObjectSlots[i].Quantity;
                    }
                }
                for (int i = 0; i < inventoryJson.EquipmentSlots.Length; i++)
                {
                    if (inventoryJson.EquipmentSlots[i] != null)
                    {
                        Inventory.EquipmentSlots[i].PickableSO = LevelManager.Instance.GetPickable(inventoryJson.EquipmentSlots[i].EffectType);
                        Inventory.EquipmentSlots[i].Quantity = inventoryJson.EquipmentSlots[i].Quantity;
                    }
                }

                InventoryToPhotography();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void GameOver()
    {
        GameManager.Instance.GameOver();
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
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
public class Pickable
{
    public PickableScriptableObject PickableSO;
    public int Quantity;
}

[Serializable]
public class PickableJSON
{
    public int Quantity;
    public EPickableEffectType EffectType;
}