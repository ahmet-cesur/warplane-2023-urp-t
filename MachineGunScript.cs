using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MachineGunScript : MonoBehaviour
{
    public Transform tr;                            // enemy plane's transform
    public Transform playerTr;                      // the player airplane object
    private Vector3 direction;                      // dir to player
    Quaternion targetRot;                           // temporary value where the gun should look at   
    [SerializeField] float turnSpeed;               // rate of rotation to target
    [SerializeField] private PrefabController prefabController;     // the script dealing bullet prefab
    [SerializeField] private GameObject Muzzleflash;        // muzzle flash on left gun exit
    [SerializeField] Transform gunParent;                           // left gun muzzle transform
    private float gunTimer;                                     // countdown to next firing time. 
    [SerializeField] private float gunCooldown;                 // time between consecutive shots
    [SerializeField] private bool firingState;                  // a primitive state machine. States firing, not firing
    public AudioSource gunSource;                           // audio source of firing guns
    public AudioClip shotClip;                              // gun shoot audio clip 
    [Tooltip("0: 360 degree Everywhere, 1: 0 degree only forward")]  
    public float spread;                                    // how wide shooting angle wil be. 0: 360 degrees everywhere, 1: 0 degrees only forward
    private void Start()
    {
        firingState = false;
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        tr= GetComponent<Transform>();
        prefabController = GameObject.FindWithTag("Canvas").GetComponent<PrefabController>();
        gunSource = GetComponent<AudioSource>();
    }
    private void FireGun()
    {
        gunTimer += Time.deltaTime;
        if(gunTimer> gunCooldown)
        {
            gunTimer = 0f;
           
        // fire l gun
            GameObject go = prefabController.GetPooledObject();// GiveBullet();
            go.transform.position = gunParent.transform.position;
            go.transform.rotation = gunParent.transform.rotation;
            go.SetActive(true);
            gunSource.PlayOneShot(shotClip);

            if (!Muzzleflash.activeInHierarchy)         // Adjust muzzle flash activity
            {
                Muzzleflash.SetActive(true);           
            }
        }
    }
    private void RotateGun()
    {
        targetRot = Quaternion.LookRotation(direction, tr.up );
        gunParent.rotation = Quaternion.Lerp(gunParent.rotation, targetRot, turnSpeed * Time.deltaTime);   
    }


    private void Update()
    {
        direction = playerTr.position - gunParent.position;
        
        if (direction.magnitude < 40f && Vector3.Dot(direction.normalized, tr.forward) >= spread)
        {
            firingState = true;
            FireGun();
            RotateGun();
        }
        if (direction.magnitude > 42f || Vector3.Dot(direction.normalized, tr.forward) < (spread - 0.01f));
        {
            firingState = false;           
        }
        


        if (!firingState)
        {   
            if (Muzzleflash.activeInHierarchy)         // Adjust muzzle flash activity
            {
                Muzzleflash.SetActive(false);
            }
        }
    }


   
}
