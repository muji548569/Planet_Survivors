using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int defaultMaxSize = 100;

    private readonly Dictionary<GameObject, GameObjectPool> pools = new();
    private Transform poolRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        poolRoot = new GameObject("ObjectPools").transform;
        poolRoot.SetParent(transform);
    }

    /// <summary>
    /// 提供給外部用來從池中獲取物件的方法
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public PoolableObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if(prefab == null)
        {
            Debug.LogError("[PoolManager] Prefab 不可為 null。");
            return null;
        }

        GameObjectPool pool = GetOrCreatePool(prefab);
        PoolableObject poolable = pool.Get(position, rotation);

        return poolable;
    }
    
    /// <summary>
    /// 預加載物件池
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="count"></param>
    public void Prewarm(GameObject prefab, int count)
    {
        if(prefab == null || count <= 0)
        {
            return;
        }

        GetOrCreatePool(prefab).Prewarm(count);
    }

    /// <summary>
    /// 由 Prefab 找到對應的池
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private GameObjectPool GetOrCreatePool(GameObject prefab)
    {
        // 如果prefab對應的池已經存在 就直接返回物件池
        if(pools.TryGetValue(prefab, out GameObjectPool pool))
        {
            return pool;
        }

        // 如果不存在就創建一個物件池
        Transform prefabPoolRoot = new GameObject($"{prefab.name}_Pool").transform;
        prefabPoolRoot.SetParent(poolRoot);

        pool = new GameObjectPool(prefab, prefabPoolRoot, defaultCapacity, defaultMaxSize);
        pools.Add(prefab, pool);
        return pool;
    }

    public void ClearAll()
    {
        foreach(GameObjectPool pool in pools.Values)
        {
            pool.Clear();
        }

        pools.Clear();
    }
}
