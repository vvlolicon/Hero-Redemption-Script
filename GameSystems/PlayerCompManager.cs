using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerCompManager : Singleton_LastIn<PlayerCompManager>
{
    private static Dictionary<Type, UnityEngine.Object> _playerComponents = new();
    private static GameObject player;

    public static GameObject Player
    {
        get
        {
            if (player == null || player.IsDestroyed())
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            if (player == null || player.IsDestroyed())
            {
                Debug.LogError("Player not found, trying to instanciate player");
                player = Instantiate(Resources.Load<GameObject>("Player"));
            }
            if (player == null)
            {
                Debug.LogError("fail to set player");
            }
            return player;
        }
        set
        {
            player = value;
        }

    }

    public static T TryGetPlayerComp<T>() where T : Component
    {
        Type type = typeof(T);
        if (_playerComponents.ContainsKey(type) && _playerComponents[type] is T comp)
        {
            if(!comp.IsCompNullOrDestroyed())
                return comp;
        }
        T component = GetPlayerT<T>();
        if (component != null)
        {
            _playerComponents[type] = component;
        }
        return component;
    }

    static T GetPlayerT<T>(Component storedComp = null) where T : Component
    {
        if (storedComp!= null && storedComp is T)
        {
            return storedComp as T;
        }

        if (Player.TryGetComponent(out T GetComponent))
        {
            return GetComponent;
        }

        var FindObjectOfTypeComp = FindObjectOfType<T>();
        if (FindObjectOfTypeComp != null)
        {
            return FindObjectOfTypeComp;
        }
        return default(T);
    }

}
