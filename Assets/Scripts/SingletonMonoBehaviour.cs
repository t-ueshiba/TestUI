using UnityEngine;

// Base class of singleton MonoBehaviour
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = (T)FindObjectOfType(typeof(T));
 
            if (_instance == null)
                Debug.LogError(typeof(T) + " is not found");

            return _instance;
        }
    }
 
    public static T InstanceNullable
    {
        get
        {
            return _instance;
        }
    }
 
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogError("Multiple " + typeof(T) + "(s) have been created", this);
            return;
        }
 
        _instance = this as T;
    }
 
    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}