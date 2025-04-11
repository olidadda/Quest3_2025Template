using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Simple Singleton pattern
    public static ObjectPooler Instance { get; private set; }

    // Define a structure for pools configurable in the Inspector
    [System.Serializable]
    public class Pool
    {
        public string tag; // Tag to identify the pool (e.g., "Water", "ContaminantA")
        public GameObject prefab;
        public int initialSize;
    }

    [Tooltip("List of object pools to initialize on start.")]
    public List<Pool> pools;

    // Dictionary to hold the actual pooled objects, keyed by tag
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    // Optional: Parent transform for pooled objects to keep Hierarchy clean
    [Tooltip("Optional: Empty GameObject to parent inactive pooled objects under.")]
    public Transform pooledObjectsParent;

    [Tooltip("Required: Transform to parent ACTIVE pooled objects under (to inherit scale/position). Assign your 'Molecules' GameObject transform here.")]
    public Transform activeObjectsParent;

    void Awake()
    {
        Debug.Log($"Parent Scale in Awake: {activeObjectsParent.lossyScale}");

        // --- Singleton Setup ---
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another instance of ObjectPooler found, destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;


        if (activeObjectsParent == null)
        {
            Debug.LogError("ObjectPooler: Active Objects Parent is not assigned! Active molecules will be root objects.", this);
        }

        
        InitializePools();
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        if (pooledObjectsParent == null)
        {
            // Create a parent if none is provided
            pooledObjectsParent = new GameObject("PooledObjects").transform;
            // Optional: Parent it to this object
            // pooledObjectsParent.SetParent(transform);
        }

        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogError($"Pool with tag '{pool.tag}' has no Prefab assigned!");
                continue;
            }

            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                //Debug.Log($"[Init] Instantiated {obj.name} #{i + 1} - Initial Local Scale: {obj.transform.localScale}, World Scale: {obj.transform.lossyScale}");
                obj.SetActive(false); // Start inactive
                obj.transform.SetParent(pooledObjectsParent, false); // Keep hierarchy clean
                objectQueue.Enqueue(obj);
            }

            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogWarning($"Pool with tag '{pool.tag}' already exists. Check configuration.");
            }
            else
            {
                poolDictionary.Add(pool.tag, objectQueue);
                Debug.Log($"Initialized pool '{pool.tag}' with {pool.initialSize} objects.");
            }
        }
    }

    /// <summary>
    /// Gets an object from the specified pool. Creates a new one if pool is empty.
    /// </summary>
    /// <param name="tag">The tag of the pool to retrieve from.</param>
    /// <param name="position">World position to spawn the object at.</param>
    /// <param name="rotation">World rotation to spawn the object with.</param>
    /// <returns>The retrieved GameObject, or null if tag not found.</returns>
    public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> queue = poolDictionary[tag];
        GameObject objectToSpawn;

        if (queue.Count > 0)
        {
            // Get from pool
            objectToSpawn = queue.Dequeue();
            //Debug.Log($"[Get] Scale when Dequeued: Local={objectToSpawn.transform.localScale} World={objectToSpawn.transform.lossyScale}");
        }
        else
        {
            // Pool empty, instantiate a new one (and find its matching prefab)
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool?.prefab != null)
            {
                //Debug.LogWarning($"Pool '{tag}' empty. Instantiating new object. Consider increasing initial size.");
                objectToSpawn = Instantiate(pool.prefab);
                //Debug.Log($"[Get] Scale after instantiated: Local={objectToSpawn.transform.localScale} World={objectToSpawn.transform.lossyScale}");
                // No need to parent this one necessarily, it's going straight into the scene active
            }
            else
            {
                Debug.LogError($"Cannot instantiate for pool '{tag}' - Prefab not found in config.");
                return null; // Should not happen if initialization worked
            }
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        //Debug.Log($"[Get] Scale before Parent: Local={objectToSpawn.transform.localScale} World={objectToSpawn.transform.lossyScale}");
        //Debug.Log($"[Get] Parent Scale before Parent: World={activeObjectsParent?.lossyScale}");

        objectToSpawn.transform.SetParent(activeObjectsParent, false);

        //Debug.Log($"[Get] Scale AFTER Parent: Local={objectToSpawn.transform.localScale} World={objectToSpawn.transform.lossyScale}");
        //Vector3 parentLocalScale = activeObjectsParent.transform.localScale;
        //objectToSpawn.transform.localScale = parentLocalScale;

        objectToSpawn.SetActive(true);

        // Optional: Call an interface method like IPooledObject.OnObjectSpawn() if needed

        return objectToSpawn;
    }


    /// <summary>
    /// Returns an object to its pool.
    /// </summary>
    /// <param name="tag">The tag of the pool the object belongs to.</param>
    /// <param name="objectToReturn">The GameObject instance to return.</param>
    public void ReturnObjectToPool(string tag, GameObject objectToReturn)
    {
        if (objectToReturn == null) return; // Ignore null objects

        //Debug.Log($"--- Returning {objectToReturn.name} ---");
        //Debug.Log($"[Return] Scale BEFORE SetActive(false): Local={objectToReturn.transform.localScale} World={objectToReturn.transform.lossyScale}");

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Trying to return object to pool '{tag}' which doesn't exist. Destroying object instead.");
            Destroy(objectToReturn);
            return;
        }
        
        objectToReturn.SetActive(false);

        //Debug.Log($"[Return] Scale AFTER SetActive(false): Local={objectToReturn.transform.localScale} World={objectToReturn.transform.lossyScale}");
        //Debug.Log($"[Return] Inactive Pool Parent Scale: World={pooledObjectsParent?.lossyScale}");


        objectToReturn.transform.SetParent(pooledObjectsParent); // Parent back for tidiness
        // Optional: Reset position/rotation/velocity if your molecules have state
        // objectToReturn.transform.position = Vector3.zero;
        // Rigidbody rb = objectToReturn.GetComponent<Rigidbody>(); if(rb) rb.velocity = Vector3.zero;

         //Debug.Log($"[Return] Scale AFTER Parenting to Inactive: Local={objectToReturn.transform.localScale} World={objectToReturn.transform.lossyScale}");


        poolDictionary[tag].Enqueue(objectToReturn);
    }
}