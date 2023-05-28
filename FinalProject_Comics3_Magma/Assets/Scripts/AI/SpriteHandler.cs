using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{

    SkeletonAnimation skeleton;

    public GameObject Graphics;
    private void Awake()
    {
        if (Graphics != null) return;
        
        skeleton = gameObject.SearchComponent<SkeletonAnimation>();
        if (skeleton)
        {
            Graphics = skeleton.gameObject;
        }
        else
        {
            Graphics = gameObject.SearchComponent<SpriteRenderer>().gameObject;
        }
    }

    private void Update()
    {
        Graphics.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
