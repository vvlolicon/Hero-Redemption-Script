using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public static int GetIndexInParent(this Transform child)
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

    public static bool IsBetweenOf(this float x, float min, float max)
    {
        return x >= min && x <= max;
    }

    public static void RoundToDecimals(this ref float value, int numDecimal)
    {
        if (value % 1 != 0)
            value = (float)Math.Round(value, numDecimal);
        
    }

    public static StoredItemPlaceType GetStoredPlaceType(this GameObject window)
    {
        if (window.CompareTag("Player_Inventory")) return StoredItemPlaceType.PlayerInventory;
        if (window.CompareTag("Player_Equipment")) return StoredItemPlaceType.PlayerEquipment;
        if (window.CompareTag("Player_HotbarItem")) return StoredItemPlaceType.PlayerHotbar;
        if (window.CompareTag("Box_Inventory")) return StoredItemPlaceType.Box;
        return StoredItemPlaceType.NONE;
    }

    public static CombatStatsType CastToCombatStatsType(this ItemAttributeName itemAttr)
    {
        return itemAttr switch
        {
            ItemAttributeName.MaxHP => CombatStatsType.MaxHP,
            ItemAttributeName.MaxMP => CombatStatsType.MaxMP,
            ItemAttributeName.HP => CombatStatsType.HP,
            ItemAttributeName.MP => CombatStatsType.MP,
            ItemAttributeName.MP_Regen => CombatStatsType.MP_Regen,
            ItemAttributeName.ATK => CombatStatsType.ATK,
            ItemAttributeName.AtkTime => CombatStatsType.AttackTime,
            ItemAttributeName.DEF => CombatStatsType.DEF,
            ItemAttributeName.SPEED => CombatStatsType.SPEED,
            ItemAttributeName.CritChance => CombatStatsType.CritChance,
            ItemAttributeName.CritChanRdc => CombatStatsType.CritChanRdc,
            ItemAttributeName.CritDmgMult => CombatStatsType.CritDmgMult,
            ItemAttributeName.CritDmgResis => CombatStatsType.CritDmgResis,
            ItemAttributeName.DmgReduce => CombatStatsType.DmgReduce,
            _ => CombatStatsType.MaxHP,
        };
    }

    public static void AddStatsRange(this GeneralCombatStats stats, Dictionary<CombatStatsType, float> addedStats)
    {
        foreach(var kvp in addedStats)
        {
            stats.ChangeStats(kvp.Key, kvp.Value);
        }
    }
    public static void AddStatsRange(this GeneralCombatStats stats, GeneralCombatStats addedStats)
    {
        var addedDict = addedStats.GetAllStats();
        AddStatsRange(stats, addedDict);
    }

    public static int GetEnumCount(this Type Enum)
    {
        return System.Enum.GetNames(Enum).Length;
    }

    public static bool IsCompNullOrDestroyed(this Component comp)
    {
        return comp == null || comp.gameObject == null || comp.IsDestroyed();
    }

    public static bool IsGameObjectNullOrDestroyed(this GameObject obj)
    {
        return obj == null || obj.IsDestroyed();
    }
}