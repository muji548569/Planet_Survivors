using System;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    // 物件回收一般是1對1 所以用 Action 而不是 Event
    private Action<PoolableObject> releaseAction;   // 回收時的委派
    private IPoolable[] poolableComponents;         // 對象身上所有實作IPoolable介面的腳本
    private bool isReleased;                        // 是否被回收到池中

    private void Awake()
    {
        poolableComponents = GetComponents<IPoolable>();
    }

    // 因為每次調用時都會直接覆蓋原本的callback 所以相比起event訂閱 更能表達唯一擁有者的概念
    /// <summary>
    /// 讓物件池把回收方法注入物件
    /// </summary>
    /// <param name="action"></param>
    public void SetReleaseAction(Action<PoolableObject> action)
    {
        releaseAction = action;
    }

    /// <summary>
    /// 從物件池取出
    /// </summary>
    public void Spawn()
    {
        isReleased = false;
        gameObject.SetActive(true);
        foreach(IPoolable poolable in poolableComponents)
        {
            poolable.OnSpawnFromPool();
        }
    }

    /// <summary>
    /// 回到物件池
    /// </summary>
    public void Release()
    {
        if(isReleased) return;

        isReleased = true;

        foreach(IPoolable poolable in poolableComponents)
        {
            poolable.OnReturnToPool();
        }

        releaseAction?.Invoke(this);
    }
}
