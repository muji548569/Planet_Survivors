using UnityEngine;

public class PersistentRoot : MonoBehaviour
{
    void Start()
    {
       DontDestroyOnLoad(gameObject);
    }
}
