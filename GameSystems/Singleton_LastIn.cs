using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton_LastIn<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    protected static bool _autoUnparentObjOnAwake = true;

    /// <summary>
    /// Gets the singleton instance of type T. If no instance exists, it attempts to find one by tag, and if that fails, it creates a new instance.
    /// </summary>
    /// <returns>The singleton instance of type T, or null if an instance could not be found or created.</returns>
    public static T Instance 
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
            }
            // does not have instant in scene
            if (_instance == null)
            {
                Debug.Log($"Cannot find the singleton instant of type{typeof(T).Name}, trying to finding one with tag");
                GameObject target = GameObject.FindAnyObjectByType<T>().gameObject;
                // if still cannot find the object, create one, normally shouldn't do this
                if (target == null)
                {
                    Debug.Log($"Cannot find the singleton instant of type{typeof(T).Name} in scene, trying to create one");
                    target = new GameObject();
                    target.name = $"Singleton {typeof(T).Name}";
                    _instance = target.AddComponent<T>();
                }
            }
            if (_instance == null)
            {
                Debug.Log($"Warning, singleton instant of type{typeof(T).Name} cannot be found or create");
                return null;
            }
            else
            {
                return _instance;
            }

        }
        set
        {
            if(_instance != null)
            {
                Debug.Log($"Destroying existing singleton {_instance} of type {typeof(T).Name}");
                Destroy(_instance);
            }
            _instance = value;
        }
    }


    private void Awake()
    {
        InitialzeSingleton();
    }

    /// <summary>
    /// Initializes the singleton instance.
    /// </summary>
    void InitialzeSingleton()
    {

        if (!Application.isPlaying)
        {
            return;
        }
        if (_instance == null)
        {
            Debug.Log($"Setting new singleton instance {this} of type {typeof(T).Name}");
        }
        Instance = this as T;
    }
}
