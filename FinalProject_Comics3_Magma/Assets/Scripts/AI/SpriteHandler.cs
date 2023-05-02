using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{

    SkeletonAnimation skeleton;

    GameObject _graphics;
    private void Awake()
    {
        skeleton = gameObject.SearchComponent<SkeletonAnimation>();
        if (skeleton)
        {
            _graphics = skeleton.gameObject;
        }
        else
        {
            _graphics = gameObject.SearchComponent<SpriteRenderer>().gameObject;
        }
    }

    private void Update()
    {
        _graphics.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }


}

public enum VectorEnum
{
    Up,
    Down,
    Right,
    Left,
    Forward,
    Back
}
