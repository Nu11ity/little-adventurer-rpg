using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance { get; private set; }
    [SerializeField] private Pool _projectilePool;
    [SerializeField] private Pool _impactPool;
    [SerializeField] private Pool _bloodPool;
    public enum PoolIdentifier
    {
        E_Projectile, E_Impact, E_Blood
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogWarning("ObjectPooler has more then one instance in this scene");

        InitializePool(_projectilePool);
        InitializePool(_impactPool);
        InitializePool(_bloodPool);
    }

    private void InitializePool(Pool pool)
    {
        if (pool.pooledObjects.Count == pool.size)
        {
            Debug.LogWarning("Notice: pool already properly sized");
            return;
        }

        try
        {
            float missingObjs = pool.size - pool.pooledObjects.Count;
            for (int i = 0; i < missingObjs; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = pool.poolParent;
                pool.pooledObjects.Add(obj);
            }
            Debug.LogWarning($"Notice: pool.pooledObjects is : {pool.pooledObjects.Count} and pool.size is : {pool.size}");
        }
        catch (System.Exception)
        {
            Debug.LogError($"ERROR: pool.pooledObjects is : {pool.pooledObjects.Count} and pool.size is : {pool.size}");
            throw;
        }
    }
    public void RequestFromPool(PoolIdentifier identifier, Vector3 position, Quaternion rotation)
    {
        switch (identifier)
        {
            case PoolIdentifier.E_Projectile: // * projectile pool
                SpawnFromPool(_projectilePool, position, rotation);
                break;
            case PoolIdentifier.E_Impact:
                SpawnFromPool(_impactPool, position, rotation);
                break;
            case PoolIdentifier.E_Blood:
                SpawnFromPool(_bloodPool, position, rotation);
                break;
        }
    }
    private void SpawnFromPool(Pool pool, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < pool.pooledObjects.Count; i++)
        {
            if (!pool.pooledObjects[i].activeInHierarchy)
            {
                pool.pooledObjects[i].transform.position = position;
                pool.pooledObjects[i].transform.rotation = rotation;
                pool.pooledObjects[i].SetActive(true);
                return;
            }
        }

        // ! -> Just remember list resizing is bad for memory. Do sparingly, plan ahead with list size...
        if (pool.canGrow)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.parent = pool.poolParent;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            pool.pooledObjects.Add(obj);
            obj.SetActive(true);
        }
    }

}
