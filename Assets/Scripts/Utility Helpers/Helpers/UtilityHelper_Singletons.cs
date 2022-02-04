using System.Collections;
using UnityEngine;


/// <summary>
/// A static instance is similar to a singletin, but instead of destroying any
/// new instances, it overrides the current instance. This is handy for resetting
/// the state and saves you doing it manually.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; protected set; }

    protected virtual void Awake() => Instance = this as T;
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic songleton. This will destroy
/// any new versions created, leaving the original instance intact.
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
            base.Awake();
        else
            Destroy(this as T);
    }
}

/// <summary>
/// This is a persistent version of the singleton which will survive through scene
/// loads. Perfect for system classes or game managers which require persistent data,
/// or audio sources where music plays through loading screens, etc.
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this as T);
    }
}