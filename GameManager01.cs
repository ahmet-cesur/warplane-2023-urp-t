
using System;
using UnityEngine;

public class GameManager01 : MonoBehaviour
{

    public GameObject player;                   // game object pane controlled by player
    public GameObject[] selectedPlanes;         // planes awailable to spawn
    public Transform spawnTransform;        // where the plane will be spawned at game start
    public Transform enemySpawnPt0;       // transform where enemy will be spawned
    public Transform enemySpawnPt1;       // alternative transform where enemy will be spawned
    public GameObject EnemySpawn0;       // what will it be spawning
    
    private void Awake()
    {
        Time.timeScale = 1f;
        int typ = PlayerPrefs.GetInt("Type", 0);   
        GameObject go = Instantiate(selectedPlanes[typ],spawnTransform.position, spawnTransform.rotation);
        go.SetActive(true);      
               
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void SpawnEnemy0()
    {
        float dist0 = (enemySpawnPt0.position-player.transform.position).sqrMagnitude;
        float dist1 = (enemySpawnPt1.position - player.transform.position).sqrMagnitude;
   
            if(dist0 < dist1)
            {
                Instantiate(EnemySpawn0, enemySpawnPt1.position, enemySpawnPt1.rotation);           
            }
            else
            {
                Instantiate(EnemySpawn0, enemySpawnPt0.position, enemySpawnPt0.rotation);           
            }     
            
           
    }
}
