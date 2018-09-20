using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T>:MonoBehaviour where T:MonoBehaviour {
    protected static T m_instance=null;
    public static T Instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<T> ();
                if(FindObjectsOfType<T> ().Length > 1) {
                    Debug.LogError (typeof (T).Name+" More than 1");
                    return m_instance;
                }
                if (m_instance == null) {
                    string instanceName = typeof (T).Name;
                    GameObject obj = new GameObject (instanceName);
                    m_instance=obj.AddComponent<T> ();
                    DontDestroyOnLoad (obj);
                }
            }
            return m_instance;
            
        }
    }
  
}
