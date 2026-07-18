using System;
using UnityEngine;

public class GamePoolInstaller : MonoBehaviour
{
    [Serializable]
    private class PrewarmEntry
    {
        public GameObject prefab;
        [Min(1)] public int count;
    }

    [SerializeField] private PrewarmEntry[] entries;

    public void Install()
    {
        if(PoolManager.Instance == null)
        {
            Debug.LogError("[GamePoolInstaller] PoolManager 不存在。");
            return;
        }

        foreach(PrewarmEntry entry in entries)
        {
            if(entry.prefab == null || entry.count <= 0)
            {
                continue;
            }
            PoolManager.Instance.Prewarm(entry.prefab, entry.count);
        }
    }
}
