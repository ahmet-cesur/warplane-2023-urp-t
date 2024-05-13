using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyPlaneScript : MonoBehaviour
{
    public CanvasScript canvasScript;
    [SerializeField] private PrefabController prefabController;     // the script dealing bullet prefab

    public Transform tr;                            // enemy plane's transform
    public Transform playerTr;                      // the player airplane object
    private Vector3 direction;                      // dir to player
    private Vector3 offset;                         // additional offset when flying to player
    Quaternion targetRot;                           // temporary value where the plane should look at   

    [SerializeField] private float speed;                            // speed of plane at that instant
    public Transform propellerTr;                   // transform of propeller
    [SerializeField] private float propSpeed;                   // turning speed of propeller
    [SerializeField] Transform gunLh;                           // left gun muzzle transform
    [SerializeField] Transform gunRh;                           // right gun muzzle transform
    private float gunTimer;                                     // countdown to next firing time. 
    [SerializeField] private float gunCooldown;                 // time between consecutive shots
    public bool isFiring;                                       // is the plane commanded to fire guns right now ?
    [SerializeField] private GameObject MuzzleflashLeft;        // muzzle flash on left gun exit
    [SerializeField] private GameObject MuzzleflashRight;       // muzzle flash on right gun exit  
    public bool isGoingToPlayer;                            // is enemy plane going to player plane;
    public Transform enemyWaypoint;                           // where is the enemy going when not going to player 
    private BonusEnemyScript bes;                           // script attached to object controlling health, score etc.
    
    private void Start()
    {
        bes = GetComponent<BonusEnemyScript>();    
        gunTimer = 0f;
        MuzzleflashLeft.SetActive(false);
        MuzzleflashRight.SetActive(false);
        canvasScript = GameObject.FindWithTag("Canvas").GetComponent<CanvasScript>();       
        prefabController = GameObject.FindWithTag("Canvas").GetComponent<PrefabController>();
        tr= gameObject.GetComponent<Transform>();
        playerTr=GameObject.FindWithTag("Player").GetComponent<Transform>();    
        isGoingToPlayer = false;        
        if (!enemyWaypoint)             // find the way point if null
        {
            enemyWaypoint = GameObject.Find("EnemyWayPt").transform;
        }
    }


    private void Update()
    {       
        propellerTr.Rotate(0f, 0f, propSpeed * Time.deltaTime, Space.Self);         // turn plane propeller
        if(bes.health > 0f)             // if enemy is not dead   
        {
            if (isGoingToPlayer)
            {
                GoToPlayer();
                CheckForFiringMuzzle();
            }
            else  // not going to player         
            {
                if (enemyWaypoint) 
                {
                    GoToWaypoint();
                }               
            }
            RotateAndMove();
        }
        else                        // if enemy is dead   
        { 
            DeadPlane();       
        }             
        if(tr.position.y < 0f)
        {
            Debug.Log("enemy fall below zero");
            bes.BonusDeadSmall();
        }
    }

    private void CheckForFiringMuzzle()
    {
        // if the player is in front of plane and is not too far away
        if (Vector3.Dot((playerTr.position - tr.position).normalized, tr.forward) > 0.95f && direction.magnitude < 15f)      
        {
            gunTimer += Time.deltaTime;
            if (gunTimer > gunCooldown)
            {
                Fire();
            }
            isFiring = true;        // for muzzle flash control
        }
        else
        {
            isFiring = false;
        }       
        if (!isFiring && MuzzleflashLeft.activeInHierarchy)       // Adjust muzzle flash activity
        {
            MuzzleflashLeft.SetActive(false);
            MuzzleflashRight.SetActive(false);
        }
    }

    private void Fire()
    {     
            // fire left side gun
            GameObject go = prefabController.GetPooledObject();// GiveBullet();
            go.transform.position = gunLh.transform.position;
            go.transform.rotation = gunLh.transform.rotation;
            go.SetActive(true);
            // fire right side gun
            go = prefabController.GetPooledObject(); // GiveBullet();
            go.transform.position = gunRh.transform.position;
            go.transform.rotation = gunRh.transform.rotation;
            go.SetActive(true);
            gunTimer = 0f;
            if (!MuzzleflashLeft.activeInHierarchy)         // Adjust muzzle flash activity
            {
                MuzzleflashLeft.SetActive(true);
                MuzzleflashRight.SetActive(true);
            }        
    }
    private void DeadPlane()     //   when health <= 0f plane is dead
    {        
        direction = tr.forward + Vector3.up * -0.7f;
        targetRot = Quaternion.LookRotation(direction, Vector3.up);
        tr.rotation = Quaternion.Lerp(tr.rotation, targetRot, 0.3f * Time.deltaTime);
        tr.position += tr.forward * speed*0.7f * Time.deltaTime;
    }
    private void GoToPlayer()
    {
        direction = playerTr.position - tr.position;
        // calculate offset    
            if (direction.magnitude < 2f)
            {
                offset = tr.right * -2f + Vector3.up * -2f;      // if enemy is too close to playet, prevent crash               
                isGoingToPlayer = false;
            }
            else
            {
                offset = Vector3.zero;                              
            }        
        if (tr.position.y < 3f)      // to prevent crash to ground
        {
            offset = Vector3.up*2f;
        }
        direction += offset;
        if (tr.position.y >320f)      // to prevent too high flying
        {
            offset = Vector3.up * -2f;
        }
        direction += offset;
        

        targetRot = Quaternion.LookRotation(direction, Vector3.up + Vector3.Dot(tr.right, direction.normalized) * tr.right * 0.5f);
    }

    private void GoToWaypoint()
    {
        direction = enemyWaypoint.position - tr.position;
        if (direction.magnitude < 5f)
        {
            isGoingToPlayer = true;
        }
        targetRot = Quaternion.LookRotation(direction, Vector3.up );
    }

    private void RotateAndMove()
    {       
        tr.rotation = Quaternion.Lerp(tr.rotation, targetRot, 0.6f * Time.deltaTime);
        tr.position += tr.forward * speed*(1f+ bes.health/bes.maxHealth)*0.5f * Time.deltaTime;

    }
    
}
