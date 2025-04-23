using System;
using System.Collections;
using System.Collections.Generic;
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

public static class ExtendIEnumerator
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

public static partial class ExtendMethods
{
    public static bool CompareItemType(this ItemType type1, ItemType type2)
    {
        if (type1 == type2)
            return true;
        else if (type1 == ItemType.Weapon || type2 == ItemType.Weapon)
        {
            if (type1 == ItemType.Weapon_Melee || type1 == ItemType.Weapon_Range || type2 == ItemType.Weapon_Melee || type2 == ItemType.Weapon_Range)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }

    public static int GetIndexOfChild(this Transform child)
    {
        if (child == null) return -1;
        Transform parent = child.parent;
        if (parent == null) return -1;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == child)
                return i;
        }
        return -1;
    }
}