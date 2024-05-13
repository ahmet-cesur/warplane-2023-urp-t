using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusEnemyScript : MonoBehaviour
{
    public bool isObjective;                                // is this enemy an objective of game? 
    public float health;                  // health of enemy object
    public float maxHealth;
    public CanvasScript canvasScript;
    [SerializeField] private int scoreWhenKilled;           // how many points when player kills this enemy
    [SerializeField] private int scoreWhenHit;              // how many points when player's bullet hit this enemy
    [SerializeField] private GameObject smallFire;          // when health is low
    [SerializeField] private GameObject bigFire;            // when health is zero
    public Collider triggerCol;                             // trigger Collider of bonua

    private void Awake()
    {
        canvasScript = GameObject.FindWithTag("Canvas").GetComponent<CanvasScript>();
        triggerCol = gameObject.GetComponent<Collider>();
        health = maxHealth;
    }
    private void Start()
    {        
        if (isObjective)
        {
            canvasScript.ObjectivesLeft += 1;
        }            
        bigFire.SetActive(false);
        smallFire.SetActive(false);     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") && health > 0f)
        {
            if (other.TryGetComponent<BulletScript>(out BulletScript bul))
            {
                health += bul.damage * -1f;
                canvasScript.GiveExplosion(other.ClosestPointOnBounds(other.transform.position));
                canvasScript.AdjScoreText(scoreWhenHit);               
                if (health < 50f && !smallFire.activeInHierarchy)
                {
                    smallFire.SetActive(true); // t
                }
                if (health <= 0f)
                {
                    BonusDead();
                }
            }
        }
    }
  

    public void BonusDead()     // killed by player's bullets 
    {
        if (isObjective)
        {
            canvasScript.ObjectiveCompleted();
        }
        triggerCol.enabled = false;
        canvasScript.MessageDisplay("Enemy destroyed", 1f);
        canvasScript.AdjScoreText(scoreWhenKilled);       
        bigFire.SetActive(true);
        Invoke("Inactivate", 15f);    
    }
    public void BonusDeadSmall()    // dead because transform.y <0
    {
        if (isObjective)
        {
            canvasScript.ObjectiveCompleted();
        }
        triggerCol.enabled = false;
        canvasScript.RefreshMarkers();
        gameObject.SetActive(false);
    }


    private void Inactivate()
    {
        canvasScript.RefreshMarkers();
        gameObject.SetActive(false);
    }
}
