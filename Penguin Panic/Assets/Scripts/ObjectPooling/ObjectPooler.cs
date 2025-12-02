using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public Transform poolRoot;

    [System.Serializable]
    public class Pool
    {
        public int id;
        //public ArrowType arrowType;
        public GameObject prefab;
        public int size = 30;
        public bool expandable = true;
    }

    public List<Pool> pools = new();
    private Dictionary<int, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;

        poolDictionary = new();

        foreach (var pool in pools)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                var obj = Instantiate(pool.prefab, poolRoot);
                obj.SetActive(false);
                var poolable = obj.GetComponent<IPoolable>();
                poolable?.OnCreatedPool();
                q.Enqueue(obj);
            }
            poolDictionary.Add(pool.id, q);
        }
    }

    //Spawn by id
    public GameObject SpawnFromPool(int idPool, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(idPool))
        {
            Debug.LogWarning($"Pool with ArrowType {idPool} doesn't exist.");
            return null;
        }

        var q = poolDictionary[idPool];
        if (q.Count == 0)
        {
            // try to find matching Pool definition to expand
            var def = pools.Find(p => p.id == idPool);
            if (def != null && def.expandable)
            {
                var extra = Instantiate(def.prefab, poolRoot);
                extra.SetActive(false);
                q.Enqueue(extra);
            }
            else return null;
        }

        var obj = q.Dequeue();
        obj.transform.SetParent(null);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnSpawnFromPool();

        // Re-enqueue so queue always has objects; this keeps a rotating buffer of instances.
        q.Enqueue(obj);

        return obj;
    }

    // Return object to pool: deactivates, parents to poolRoot and calls OnReturnToPool
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        var poolable = obj.GetComponent<IPoolable>();
        poolable?.OnReturnToPool();

        obj.SetActive(false);
        obj.transform.SetParent(poolRoot);
    }
}