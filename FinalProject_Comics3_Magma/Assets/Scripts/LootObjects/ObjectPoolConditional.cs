using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolConditional : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawnPrefab;
    [SerializeField] Transform spawnPivot;
    GameObject objectSpawned;

    public void CheckAndSpawn()
    {
        if(objectSpawned == null && !GameManager.Instance.Player.PlayerManager.HasObjectInInventory(EPickableEffectType.ThrowBomb))
        {
            objectSpawned = Instantiate(objectToSpawnPrefab, spawnPivot.position, Quaternion.identity);
        }
    }
}
