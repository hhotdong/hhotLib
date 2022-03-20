//using System.Collections;
//using UnityEngine;

//public class MiniObjectPool<T> : MiniObjectPool
//{
//    public T m_PoolType;
//}

//public class MiniObjectPool : MonoBehaviour
//{
//    protected Transform tr;

//    [Header("Pool Infos")]
//    protected ObjectPool m_ObjectPool;

//    [SerializeField] protected GameObject m_PoolObject;
//    [SerializeField] protected int m_PoolAmount;

//    protected static readonly int DEFAULT_POOL_AMOUNT = 2;


//    //////////////////////////////////////////
//    // Initialize
//    //////////////////////////////////////////

//    private void Awake()
//    {
//        tr = GetComponent<Transform>();
//        this.gameObject.name = string.Format("{0}{1}", "MiniObjectPool_", m_PoolObject.name);

//        StartCoroutine(InitObjectPool());
//    }

//    protected virtual IEnumerator InitObjectPool()
//    {
//        m_ObjectPool = new ObjectPool();
//        m_ObjectPool.source = m_PoolObject;
//        m_ObjectPool.folder = this.gameObject;

//        int amount = m_PoolAmount > 0 ? m_PoolAmount : DEFAULT_POOL_AMOUNT;

//        for (int i = 0; i < amount; i++)
//        {
//            m_ObjectPool.unusedList.Add(CreatePoolObject(true));
//            yield return new WaitForEndOfFrame();
//        }

//        m_ObjectPool.maxAmount = amount;
//    }

//    public GameObject Get()
//    {
//        if (m_ObjectPool.unusedList.Count > 0)
//        {
//            GameObject obj = m_ObjectPool.unusedList[0];
//            m_ObjectPool.unusedList.RemoveAt(0);
//            obj.SetActive(true);
//            return obj;
//        }
//        else
//            return CreatePoolObject(false);
//    }

//    protected virtual GameObject CreatePoolObject(bool isInit)
//    {
//        GameObject obj = Instantiate(m_ObjectPool.source);
//        obj.SetActive(!isInit);
//        obj.transform.SetParent(m_ObjectPool.folder.transform, false);

//        return obj;
//    }

//    public void Free(GameObject obj)
//    {
//        string keyName = obj.transform.parent.name;
//        if (!string.Equals(keyName, m_ObjectPool.folder.name))
//        {
//            Debug.LogError($"Failed to free object because this object doesn't belong to this folder({keyName})!");
//            return;
//        }

//        obj.SetActive(false);
//        m_ObjectPool.unusedList.Add(obj);
//    }
//}
