
using UnityEngine;


public class PlayerPlaneScript : MonoBehaviour
{
    public Animator animator;
    public CanvasScript canvasScript;
    [SerializeField] private PrefabController prefabController;     // the script dealing bullet prefab
    public Transform tr;                            // plane transform
    public Vector2 dir;                             // basically joystick input
    public Vector3 direction3D;                     // a 3d vector for plane forward direction  
    Quaternion targetRot;                           // temporary value where the plane should look at   
    float speed;                                    // speed of plane at that instant
    public float agility;                           // how fast can change direction
    [SerializeField] private float maxHealth;       // maximum health value when level loads or regeneration
    [SerializeField] private float health;                           // health variable of plane. 0 when dead   
    public float speedRatio;                        // speed lever value from canvas
    public float maxSpeedHigh;                      // max speed with landing gear closed - faster
    public float maxSpeedLow;                       // max speed with landing gear open
    private float maxSpeed;                         // the max speed depending on landing gear pos
    private float gravityEffect;                    // plane goes faster when nose turned downwards 
    public Transform MeshTransform;                 // the mesh renderer part of plane
    public float fallingSpeed;                      // plane falls when speed is low - 0.2 x maxspeed
    public float altitude;                          // distance from plane bottom to ground
    public Transform planeBottom;                   // where raycast will measure altitude from
    public LayerMask layerMask;                     // raycast height measurement raycast apply to which layers
    public bool grounded;                           // is plane landed on ground or moving on wheels on grnd
    [SerializeField] private float groundClearance;                  // plane tr to raycast hit point
    bool landingGearOpen;                           // are wheels of plane at down position - opened
    public int snapY;                               //  vertical controls snap-reverse direction 
    [SerializeField] Transform gunLh;               // left gun muzzle transform
    [SerializeField] Transform gunRh;               // right gun muzzle transform
    private float gunTimer;                                 // countdown to next firing time. 
    [SerializeField] private float gunCooldown;             // time between consecutive shots  
    public bool isFiring;                                   // is the plane commanded to fire right now ?
    [SerializeField] private GameObject MuzzleflashLeft;    // muzzle flash on left gun exit
    [SerializeField] private GameObject MuzzleflashRight;   // muzzle flash on right gun exit
    [SerializeField] private GameObject explosionBig;       // when plane dies
    [SerializeField] private GameObject smallFire;          // when health is low
    public TrailRenderer trailLeft;                         // trail renderer on wing tip left side
    public TrailRenderer trailRight;                        // trail renderer on wing tip right side   
    [SerializeField] private ParticleSystem exhaustLh;          // engine exhaust left side
    [SerializeField] private ParticleSystem exhaustRh;          // engine exhaust right side
    // this scripted is used by Warplane project
    public int waitIncomplete;                                    // wait a few frames before fixed update when game loads to prevent crashing to terrain.
    private bool fireSecondGun=false;                       // to fire the second gun after 1 frame
    public AudioSource engineSource;                        // audio source for engine sound
    public AudioSource gunClipSource;                       // audio source for plane's gun    
    public AudioClip shotClip;                              // audio clip of gun shot sound 
    [SerializeField] private bool isDead;                            // a primitive check if plane is dead or not for taking damage
    public float planeLift;                                 // how high must be distance from "bottom" component to rayscan hit

    void Start()
    {
        isDead = false;
        canvasScript = GameObject.FindWithTag("Canvas").GetComponent<CanvasScript>();
        prefabController = GameObject.FindWithTag("Canvas").GetComponent<PrefabController>();
        animator = GetComponent<Animator>();
        landingGearOpen = true;
        maxSpeed = maxSpeedLow;       
        snapY = PlayerPrefs.GetInt("SnapY", 1);
        gunTimer = 0f;
        MuzzleflashLeft.SetActive(false);
        MuzzleflashRight.SetActive(false);
        explosionBig.SetActive(false);
        smallFire.SetActive(false);
        health = maxHealth;
        canvasScript.AdjHealthSlider(health / maxHealth);    
        trailLeft.emitting = false;
        trailRight.emitting = false;
        grounded = true;
        RayCheck();    
    }
  

    private void Update()
    {
        engineSource.pitch = 0.7f + speed/maxSpeedLow*0.4f;
        if (waitIncomplete > 0)
        {
            waitIncomplete--;
            return;
        }
        if (!isDead)
        {
            if (tr.position.y < -0.5f)       //  crashed       
            {
                PlaneDeath("You crashed", 10f);
            }
            direction3D = tr.up * dir.y * 0.4f + tr.right * dir.x * 0.3f + tr.forward;                              // from joystick input to 3d lookat vector

        
            if (grounded)
            {
                GroundState();
            }
            else  // to avoid stopping in airborne
            {
                FlyingState();
            }


            if (isFiring && canvasScript.bulletsLeft > 1)
            {
                gunTimer += Time.deltaTime;
                FireSecondGun();
                Fire03();
                canvasScript.BulletAdjust();
            }
            if (!isFiring && MuzzleflashLeft.activeInHierarchy)
            {
                MuzzleflashLeft.SetActive(false);
                MuzzleflashRight.SetActive(false);
            }
            
            RegenHealth();
            SpeedGauge();
            CheckExhausts();
        }
        if (isDead)
        {
            if (speed < 5f)
            {
                fallingSpeed = 0.3f * (5f - speed) * (5f - speed);
            }
            else
            {
                fallingSpeed = 0f;
            }
            tr.position += (fallingSpeed * Vector3.up * -1f + speed * tr.forward) * Time.deltaTime;
           // tr.position += speed * tr.forward * Time.deltaTime;
        }
    }
 

 
    private void GroundState()
    {
  
        speedRatio = Mathf.Clamp(speedRatio, -0.1f, 0.6f);                                                              // speed ratio is adjusted by lever on canvas
        speed = Mathf.Lerp(speed, speedRatio * maxSpeed, 0.2f * Time.deltaTime);
        if (speed < 0f)
        {
            speed = 0f;
        }
        direction3D.x = Mathf.Clamp(direction3D.x, -0.1f, 0.1f);
        direction3D.y = Mathf.Clamp(direction3D.y, 0.01f, 0.5f);
        if (speed < 5f)                                                                                                 // when plane is moving on ground slowly, can not take off
        {
            direction3D.y = 0f;
        }
        else
        {
            direction3D.y = Mathf.Clamp(direction3D.y, 0f, 0.5f);                                                       // when plane is moving on ground faster it can take off
        }
        targetRot = Quaternion.LookRotation(direction3D, Vector3.up);                                                   // this line helps yaw the plane when grounded
        tr.position += speed * direction3D * Time.deltaTime;                                                            // 0.25 is height from ground when wheels look ok         
        if (groundClearance < planeLift)                                                                                    // to make wheels position on ground level
        {
            tr.position += (planeLift +0.01f - groundClearance) * Vector3.up * Time.deltaTime * 25f;                               // 0.25 is height from ground when wheels look ok
        }

        AdjTailAngle();                                                                                                 // tail of plane lifts at certain speed
        tr.rotation = Quaternion.Lerp(tr.rotation, targetRot, 2f * Time.deltaTime);
    }
    private void FlyingState()
    {
        speedRatio = Mathf.Clamp(speedRatio, 0.05f, 1f);
        if (tr.rotation.eulerAngles.x > 180)            // moves faster when descending
        {
            gravityEffect = 1f + 0.005f * (tr.rotation.eulerAngles.x - 360f);
        }
        else
        {
            gravityEffect = 1f + 0.005f * (tr.rotation.eulerAngles.x);
        }
        speed = Mathf.Lerp(speed, speedRatio * maxSpeed * gravityEffect, 0.4f * Time.deltaTime);

        TrailControl();
        
        // drop plane down when speed is below 5 m/s
        if (speed < 5f)
        {
            fallingSpeed = 0.3f * (5f - speed)* (5f - speed);
        }
        else 
        { 
            fallingSpeed = 0f;
        }       
        if (tr.position.y > 80f)                                                                    // check for extreme high altitude
        {
            direction3D.y = Mathf.Clamp(direction3D.y, -0.5f, -0.05f);
        }
        else
        {
            direction3D.y = Mathf.Clamp(direction3D.y, -0.5f, 0.5f);
        }
        if (dir.x != 0f )
        {
            targetRot = Quaternion.LookRotation(direction3D, tr.up + tr.right * dir.x * 0.6f);       // this line helps yaw the plane when flying
        }
        else
        {
            targetRot = Quaternion.LookRotation(direction3D, Vector3.up);                           // this line helps return the plane to parallel
        }
        tr.position += (fallingSpeed * Vector3.up * -1f + speed * tr.forward ) * Time.deltaTime;
        tr.rotation = Quaternion.Lerp(tr.rotation, targetRot, agility * Time.deltaTime);
    }
    private void FixedUpdate()
    {
        if (waitIncomplete > 0)
        {            
            return;
        }
        RayCheck();    
    }

    private void RayCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(planeBottom.position, planeBottom.TransformDirection(Vector3.up * -1f), out hit, 5f, layerMask))
        {
         
            if (hit.distance < planeLift + 0.02f)
            {
                groundClearance = hit.distance;               
                grounded = true;

                if (!landingGearOpen)
                {
                    PlaneDeath("You crashed your plane.<br> Open landing gear when descending", 10f);
                }
                else if (hit.distance < 0.5f)    // too fast landing
                {
                    PlaneDeath("You crashed your plane "+hit.distance, 10f);  // landing gear can be opened here
                }
            }
            if (hit.distance > planeLift + 0.021f)
            {
                grounded = false;
            }
        }
        else 
        {
            grounded = false;
        }
    }
    private void TrailControl()     // to control trail emission at wing tips
    {
        if (!trailLeft.emitting)
        {
            //if (speed > 9.1f && Mathf.Abs(dir.y) < 0.8f)         // start trail emitting
                if (speed > 9.1f )
                {
                trailLeft.emitting = true;
                trailRight.emitting = true;
            }
        }
        if (trailLeft.emitting)
        {
            if (speed < 9f )         // stop trail emitting
            {
                trailLeft.emitting = false;
                trailRight.emitting = false;
            }
        } 
    }

    private void PlaneDeath(string message, float t)
    {
        explosionBig.SetActive(true);   
        canvasScript.GiveExplosion(tr.position);
        canvasScript.GameOverMessage("GAME OVER <br>"+message, t);
        speedRatio = 0f;
    }
    private void RegenHealth()
    {
        health += Time.deltaTime;
        if(health >maxHealth)
        {
            health = maxHealth;
        }
        canvasScript.AdjHealthSlider(health/maxHealth);
        if (health > maxHealth * 0.5f && smallFire.activeInHierarchy)
        {
            smallFire.SetActive(false);
        }
    }

    public void Fire03()
    {
     
        if (gunTimer > gunCooldown)
        {
            // fire left side gun
         
            GameObject go = prefabController.GetPooledObject();
            go.transform.position = gunLh.transform.position;
            go.transform.rotation = gunLh.transform.rotation;
            go.SetActive(true);
            gunClipSource.PlayOneShot(shotClip);

            // fire right side gun
           // go = prefabController.GiveBullet();
            
            gunTimer = 0f;
            canvasScript.bulletsLeft += -1;
            canvasScript.BulletAdjust();
            if (!MuzzleflashLeft.activeInHierarchy)         // Adjust muzzle flash activity
            {
                MuzzleflashLeft.SetActive(true);
                MuzzleflashRight.SetActive(true);
            }
            fireSecondGun=true;            
        }
    }

    private void FireSecondGun()
    {
        if (fireSecondGun)
        {
            GameObject  go = prefabController.GetPooledObject();
            go.transform.position = gunRh.transform.position;
            go.transform.rotation = gunRh.transform.rotation;
            go.SetActive(true);
            fireSecondGun =false;
            canvasScript.bulletsLeft += -1;
            canvasScript.BulletAdjust();
        }
    }

    #region Tail lift


    private void AdjTailAngle()
    {
        if (speed < maxSpeed * 0.5f && speed > maxSpeed * 0.4)                                    // here I adjust angle of hull related to gameobject. in slow spaad rear wheel touches ground
        {
            MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.LookRotation(Vector3.forward + 3f * Vector3.up * (0.5f - speed / maxSpeed)), 0.05f);
        }
        if (speed < maxSpeed * 0.4f)
        {
            MeshTransform.localRotation = Quaternion.Lerp(MeshTransform.localRotation, Quaternion.LookRotation(Vector3.forward + 0.3f * Vector3.up), 0.1f);
        }

    }

    #endregion
    private void SpeedGauge()
    {
        canvasScript.AdjSpeedText("Spd: " + Mathf.Round(speed * 12f) + " km/h", "Alt: " + Mathf.Round(tr.position.y * 4f) + " m");
    }
    public void OpenLandingGear()
    {
        animator.SetTrigger("openIt");
        landingGearOpen = true;
        maxSpeed = maxSpeedLow;
    }

    public void CloseLandingGear()
    {       
            animator.SetTrigger("closeIt");
            landingGearOpen = false;
            maxSpeed = maxSpeedHigh;     
    }

    public void AdjDirection(float x1, float y1)  // this function is accessed by Canvasscript to copy joystick input to plane  
    {
        dir.x = x1;
        dir.y = y1 * snapY;

        if (Input.GetAxisRaw("Horizontal") != 0f)
        {
            dir.x = Input.GetAxis("Horizontal");
        }
        if (Input.GetAxisRaw("Vertical") != 0f)
        {
            dir.y = Input.GetAxis("Vertical") * snapY;
        }

    }

    public void AdjPower(float f) // this function is accessed by Canvasscript to copy throttle power SLIDER input to plane  
    {
        speedRatio = f;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.CompareTag("Bullet"))
            {
                if (other.TryGetComponent<BulletScript>(out BulletScript bul))
                {
                    health += bul.damage * -1f;
                    canvasScript.AdjHealthSlider(health / maxHealth);
                    if (health < maxHealth * 0.4f)
                    {
                        smallFire.SetActive(true);
                    }
                    if (health < 0f)
                    {
                        PlaneDeath("Killed In Action", 10f);
                        isDead = true;
                    }
                }
            }
            else
            {
                string st = other.tag;
                if (st == "EnemyPlane")
                {
                    st = "an enemy plane";
                }
                PlaneDeath("You crashed your plane to " + st, 10f);
                isDead = true;
            }
        }
    }
    private void CheckExhausts()
    {
        if (speed < 5f && !exhaustLh.isPlaying)        // exhaust particle emit only below certain speeds for realism.
        {
            exhaustLh.Play();
            exhaustRh.Play();
        }
        if (speed > 5.5f && exhaustLh.isPlaying)
        {
            exhaustLh.Stop();
            exhaustRh.Stop();
        }
    }
}
