using UnityEngine;

//シングルトンなMonoBehaviourの基底クラス
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
                Debug.LogError(typeof(T) + "is nothing");

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
            Debug.LogError(typeof(T) + " is multiple created", this);
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