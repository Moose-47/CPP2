using UnityEngine;

[DefaultExecutionOrder(-1)]
//abstract is a class that can't be directly instantiated but children of it can be used. <T> is a generic template, in this case T is Component
public abstract class Singleton<T> : MonoBehaviour where T : Component 
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                GameObject obj = new GameObject($"{typeof(T).Name}");
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    //virtual allows the function to be overridden by child classes
    protected virtual void Awake()
    {
        if (!instance)
        {
            instance = this as T;
            DontDestroyOnLoad(instance);
            return;
        }

        Destroy(gameObject);
    }
}
