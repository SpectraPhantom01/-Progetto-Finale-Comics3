using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlime : MonoBehaviour
{
    [SerializeField] float timeSingleTrailLife;
    [SerializeField] float deltaTimeSpawnTrail;
    [SerializeField] GameObject trailPrefab;

    float timePassed = 0;

    private void Update()
    {
        timePassed += Time.deltaTime * 1;
        if(timePassed >= deltaTimeSpawnTrail)
        {
            var trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
            Destroy(trail, timeSingleTrailLife);
            timePassed = 0;
        }
    }
}
