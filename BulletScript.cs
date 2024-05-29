using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletScript : MonoBehaviour
{
    private Vector3 shootDirection;                                         // direction when exited muzzle after randomization of direction
    [SerializeField] private float speed;                                   // variable bullet speed at any time
    public float exitSpeed;                                                 // speed when exiting muzzle
    [SerializeField] private float gravitySpeedComponent;                   // force applied by gravity. makes trajectory move downwards
    [SerializeField] private float maxGravity;                              // the maximum value gravity can reach
    private Transform tr;                                                   // transform of this bullet
    private float timer;                                                    // how much time left before auto disable;
    public float lifeNominalLength;                                         // a fixed value for length of life before deactivation. this will be used for calculating random real life length
    private float randomLife;                                               // each bullet will have a random life span related to nominal
    public float maxDamage;                                                 // maximum damage inflicted when hit an actor
    public float damage;                                                    // how much damage inflicted when hit an actor. corrected for difficulty
    float devy = 0f;                                                        // random deviation in direction Y
    float devx = 0f;                                                        // random deviation in direction X


    private void Awake()
    {
        randomLife = lifeNominalLength * Random.Range(0.8f, 1.5f);  // to give each bullet a arandomized life expectancy
        tr= GetComponent<Transform>();
        int difficulty;
        difficulty = PlayerPrefs.GetInt("Diff", 0);
        switch (difficulty)
        {
            case 1:
                damage = maxDamage * 0.8f;       // test for not looping later
                break;
            case 2:
                damage = maxDamage * 0.6f;
                break;
            case 0:
                damage = maxDamage;
                break;
            default:
                damage = maxDamage;
                break;
        }
    }
    private void OnEnable()
    {
        timer = 0f;        
        gravitySpeedComponent = 0f;
        speed=exitSpeed;
        devx = Random.Range(-0.2f, 0.2f);
        devy = Random.Range(-0.3f, 0.3f);
        gameObject.transform.Rotate(devy, devx, 0f, Space.World); // devy vertical, devx horizontal deviation
        shootDirection = transform.forward;
    }

    private void Update()
    {
        if(gameObject.activeInHierarchy)
        {
            speed = exitSpeed*((lifeNominalLength- timer)*0.5f/lifeNominalLength +0.5f);
            if (gravitySpeedComponent < maxGravity)     // adjust gravitational pull downwards
            {
                gravitySpeedComponent += 0.5f * Time.deltaTime;
            }
            tr.position += (shootDirection * speed - gravitySpeedComponent * Vector3.up) * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer > randomLife)
            {
                timer = 0f;
                gameObject.SetActive(false);                     
            }
        }

        if(tr.position.y > 80f || tr.position.y < -2f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {              
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {              
        gameObject.SetActive(false);

    }
}
