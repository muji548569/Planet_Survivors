using UnityEngine;
using UnityEngine.Pool;
public class GameObjectPool
{
    private readonly GameObject prefab;
    private readonly Transform poolRoot;
    private readonly ObjectPool<PoolableObject> pool;

    public GameObjectPool(GameObject prefab, Transform poolRoot, int defaultCapacity = 10, int maxSize = 100)
    {
        this.prefab = prefab;
        this.poolRoot = poolRoot;

        pool = new ObjectPool<PoolableObject>(
            createFunc: CreateObject,
            actionOnGet: null,
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

    private void OnReleaseObject(PoolableObject poolable)
    {
        poolable.transform.SetParent(poolRoot);
        poolable.gameObject.SetActive(false);
    }

    private void OnDestroyObject(PoolableObject poolable)
    {
        Object.Destroy(poolable.gameObject);
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

    public void Clear()
    {
        pool.Clear();
    }
}
