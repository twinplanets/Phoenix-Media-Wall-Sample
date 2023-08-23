using UnityEngine;
using System.Collections;

//Generic Singleton classes for Unity. Use MonoBehaviourSingletonPersistent for any singleton that must survive a level load. 
public class MonoBehaviourSingleton<T> : MonoBehaviour
	where T : Component
{
    private static T _instance;

    public static T Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this.GetComponent<T>();
        }
    }
}
