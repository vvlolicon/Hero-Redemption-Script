using System;
using System.Collections;
using UnityEngine;

public static class ExtendVector3 
{
    public enum Axis { X, Y, Z }

    public static Vector3 ChangeAxisValue(this Vector3 vector, Axis axis, float value)
    {
        return axis switch
        {
            Axis.X => new Vector3(value, vector.y, vector.z),
            Axis.Y => new Vector3(vector.x, value, vector.z),
            Axis.Z => new Vector3(vector.x, vector.y, value),
            _ => vector,
        };
    }
}

public static class ExtendMethods
{
    public static IEnumerator DelayAction(float delay, Action todo)
    {
        yield return new WaitForSeconds(delay);
        todo();
    }

    public static IEnumerator ActionInNextFrame(Action todo)
    {
        yield return new WaitForEndOfFrame();
        todo();
    }
}