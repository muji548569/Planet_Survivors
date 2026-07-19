using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class GameObjectPool
{
    private readonly GameObject prefab;
    private readonly Transform poolRoot;
    private readonly ObjectPool<PoolableObject> pool;
    // 用於追蹤active狀態的物件
    private readonly HashSet<PoolableObject> activeObjects = new();

    public GameObjectPool(GameObject prefab, Transform poolRoot, int defaultCapacity = 10, int maxSize = 100)
    {
        this.prefab = prefab;
        this.poolRoot = poolRoot;

        pool = new ObjectPool<PoolableObject>(
            createFunc: CreateObject,
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
            );
    }

    private PoolableObject CreateObject()
    {
        GameObject instance = Object.Instantiate(prefab, poolRoot);
        instance.name = prefab.name;
        PoolableObject poolable = instance.GetComponent<PoolableObject>();
        if (poolable == null)
        {
            poolable = instance.AddComponent<PoolableObject>();
        }
        poolable.SetReleaseAction(pool.Release);
        instance.SetActive(false);
        return poolable;
    }

    private void OnGetObject(PoolableObject poolable)
    {
        activeObjects.Add(poolable);
    }

    private void OnReleaseObject(PoolableObject poolable)
    {
        activeObjects.Remove(poolable);
        poolable.gameObject.SetActive(false);

        if(poolRoot != null)
        {
            poolable.transform.SetParent(poolRoot);
        }
    }

    private void OnDestroyObject(PoolableObject poolable)
    {
        activeObjects.Remove(poolable);

        if (poolable != null)
        {
            Object.Destroy(poolable.gameObject);
        }
    }

    public PoolableObject Get(Vector3 position, Quaternion rotation)
    {
        PoolableObject poolable = pool.Get();

        poolable.transform.SetPositionAndRotation(position, rotation);
        poolable.Spawn();

        return poolable;
    }

    public void Prewarm(int count)
    {
        PoolableObject[] objects = new PoolableObject[count];

        for (int i = 0; i < count; i++)
        {
            objects[i] = pool.Get();
        }

        foreach (PoolableObject poolable in objects)
        {
            pool.Release(poolable);
        }
    }

    private void ReleaseAllActive()
    {
        // 必須要先 copy 再 release 會觸發 OnReleaseObject
        // 裡面有 activeObjects.Remove(poolable); 
        // 這相當於遍歷集合時修改集合，會拋出例外
        PoolableObject[] objects = new PoolableObject[activeObjects.Count];
        activeObjects.CopyTo(objects);
        foreach(PoolableObject poolable in objects)
        {
            if(poolable != null) 
                poolable.Release();
        }

        activeObjects.Clear();
    }

    public void Clear()
    {
        ReleaseAllActive();
        pool.Clear();
    }
}
