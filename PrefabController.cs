using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
    private GameObject go;    
    public GameObject prefabToPool;
    public int initialPoolSize = 30;
    private List<GameObject> objectPool; 
    // this scripted is used by Warplane project to create and give bullets to player plane and enemy plane

    private void Start()
    {
        // Initialize the pool
        objectPool = new List<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreatePooledObject();
        }
    }
    public GameObject GetPooledObject()
    {
        foreach (var obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
               // obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive objects found, create a new one
        var newObj = CreatePooledObject();
        newObj.SetActive(true);
        return newObj;
    }
    // Create a new pooled object
    private GameObject CreatePooledObject()
    {
        var newObj = Instantiate(prefabToPool);
        newObj.SetActive(false);
        objectPool.Add(newObj);
        return newObj;
    }
}
