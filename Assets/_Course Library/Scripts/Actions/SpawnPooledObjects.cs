using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Create a pool of prefab objects, then allow the user to spawn one of those objects or remove them all
/// This is more performant and less prone to crashing than instantiating & destroying objects.
/// </summary>
public class SpawnPooledObjects : MonoBehaviour
{
    [Tooltip("The object prefab you want to put into the object pool")]
    public GameObject pooledObject;

    [Tooltip("The transform where the object will be spanwed")]
    public Transform spawnPosition = null;

    [Tooltip("The number of objects you want in the object pool")]
    public int pooledAmount = 50;

    [Tooltip("A display of the list of objects in the pool")]
    public List<GameObject> pooledObjects;


    void Start()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            obj.transform.parent = this.transform;
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

    public void SpawnPooledObject()
    {
        GameObject newObj = GetPooledObject();
        newObj.SetActive(true);
        newObj.transform.SetPositionAndRotation(spawnPosition.position, spawnPosition.rotation);
    }

    public void ClearAllPooledObjects()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i] != null)
            {
                pooledObjects[i].SetActive(false);
            }
        }
    }

    private void OnValidate()
    {
        if (!spawnPosition)
            spawnPosition = transform;
    }

}