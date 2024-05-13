using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
  //  public GameObject bulletPrefab;                   // prefab of bullet
   // private GameObject[] bullets;
  //  public int totalBullets;                    // how many bullets to instantiate
    private GameObject go;
    
    public GameObject prefabToPool;
    public int initialPoolSize = 30;
    private List<GameObject> objectPool;

    // this scripted is used by Warplane project
   /* private void Awake()
    {
       GameObject go1;
        bullets = new GameObject[totalBullets];
        for(int i=0; i<totalBullets; i++)
        {
            go1 = Instantiate(bulletPrefab);
            bullets[i] = go1;
            bullets[i].SetActive(false);
        }       
    }*/
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

  /*  public GameObject GiveBullet()
    {       
        for (int i = 0; i < totalBullets; i++)
        {
            if (!bullets[i].activeInHierarchy)
            {
                go= bullets[i];
            }        
        }        
        return go;
    }         */
}
