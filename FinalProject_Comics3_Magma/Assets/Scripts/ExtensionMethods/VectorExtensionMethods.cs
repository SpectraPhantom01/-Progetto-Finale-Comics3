using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensionMethods
{
    public static EDirection CalculateDirection(this Vector2 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)) // horizontal priority
        {
            if (velocity.x > 0)
            {
                return EDirection.Right;
            }
            else
            {
                return EDirection.Left;
            }
        }
        else // vertical priority
        {
            if (velocity.y > 0)
            {
                return EDirection.Up;
            }
            else
            {
                return EDirection.Down;
            }
        }
    }


    public static EDirection CalculateDirection(this Vector3 velocity)
    {
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)) // horizontal priority
        {
            if (velocity.x > 0)
            {
                return EDirection.Right;
            }
            else
            {
                return EDirection.Left;
            }
        }
        else // vertical priority
        {
            if (velocity.y > 0)
            {
                return EDirection.Up;
            }
            else
            {
                return EDirection.Down;
            }
        }
    }
}
