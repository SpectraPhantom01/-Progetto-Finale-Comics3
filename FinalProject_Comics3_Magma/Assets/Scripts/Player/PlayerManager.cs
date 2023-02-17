using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour, IAliveEntity
{
    [Header("Hourglass Settings")]
    [SerializeField] float timeLoseDustInHourglass;
    [SerializeField] float amountLoseDust;
    [Header("Attack Settings")]
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;
    [Header("Inventory Settings")]
    public Inventory Inventory;

    public EDirection CurrentDirection = EDirection.Down;
    public bool IsAlive { get ; set ; }
    public string Name => "Knight of Time";
    public Damageable Damageable => _damageable;

    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public PlayerController PlayerController => _playerController;
    private float hourglassTimePassed;
    private Damageable _damageable;
    private PlayerController _playerController;

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
    }

    private void Update()
    {
        HandleHourglass();

        if(_playerController.Rigidbody.velocity.magnitude > 0.01f)
            CurrentDirection = _playerController.Rigidbody.velocity.CalculateDirection();
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if(hourglassTimePassed >= timeLoseDustInHourglass)
        {
            hourglassTimePassed = 0;
            //_damageable.Damage(amountLoseDust);
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