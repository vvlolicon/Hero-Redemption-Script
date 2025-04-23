using System.Collections.Generic;
using System;
using UnityEngine;

public class GameObjectManager : Singleton<GameObjectManager>
{
    private static Dictionary<Type, UnityEngine.Object> _playerComponents = new();

    public static T TryGetPlayerComp<T>() where T : UnityEngine.Object
    {
        Type type = typeof(T);
        if (_playerComponents.ContainsKey(type) && _playerComponents[type] is T t)
        {
            return t;
        }
        else
        {
            T component = GetPlayerT<T>(null);
            if (component != null)
            {
                _playerComponents[type] = component;
            }
            return component;
        }
    }

    static T GetPlayerT<T>(Component storedComp) where T : UnityEngine.Object
    {
        if (storedComp is T)
        {
            return storedComp as T;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return null;
        }

        var GetComponent = player.GetComponent<T>();
        if (GetComponent != null)
        {
            return GetComponent;
        }

        var FindObjectOfTypeComp = FindObjectOfType<T>();
        if (FindObjectOfTypeComp != null)
        {
            return FindObjectOfTypeComp;
        }

        Debug.LogError("Component not found!");
        return null;
    }
}
