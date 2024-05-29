using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusEnemyScript : MonoBehaviour
{
    public bool isObjective;                                // is this enemy an objective of game? 
    public float health;                                    // health of enemy object
    public float maxHealth;                                 // the beginning max health
    public CanvasScript canvasScript;
    [SerializeField] private int scoreWhenKilled;           // how many points when player kills this enemy
    [SerializeField] private int scoreWhenHit;              // how many points when player's bullet hit this enemy
    [SerializeField] private GameObject smallFire;          // when health is low
    [SerializeField] private GameObject bigFire;            // when health is zero
    public Collider triggerCol;                             // trigger Collider of bonus

    private void Awake()
    {
        canvasScript = GameObject.FindWithTag("Canvas").GetComponent<CanvasScript>();
        triggerCol = gameObject.GetComponent<Collider>();
        health = maxHealth;
    }
    private void Start()
    {       
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
                if (health < maxHealth*0.4f && !smallFire.activeInHierarchy)
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
        Invoke("Inactivate", 15f);    // wait until altitude is too low. such as wait zeppelin fall slowly
    }
    public void BonusDeadSmall()    // dead because transform.y <0 and not killed by player
    {   
        triggerCol.enabled = false;    
        Destroy(gameObject, 2f);
    
    }


    private void Inactivate()
    {     
        Destroy(gameObject, 2f);     
    }
}
