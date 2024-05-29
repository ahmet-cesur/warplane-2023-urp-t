

using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
//using System.Drawing;


[RequireComponent(typeof(GameManager01))]
public class CanvasScript : MonoBehaviour
{

    [SerializeField] private GameManager01 gameManager;     // game manager for spawning player and enemies
    public GameObject player;                   // game object pane controlled by player
    public PlayerPlaneScript playerPlane;       // script of player controlled plane
    public VirtualCamSwitcher virtCamSwitcher;  // a script attached to player. used for switching between virt cameras
    public Image blackoutImage;                 // decrease opacity when game starts;
    public float fadeTime = 2.0f;               // Duration of the fade (in seconds)
    private bool gearOpen;                      // is landing gear opened. opened = landed position
    public TMP_Text gearButtonText;             // text on button for opening gears
    public TMP_Text speedText;                  // speed of plane
    public TMP_Text altText;                    // altitude of plane. position.y comp   
    public TMP_Text messageText;                // messages shown at various occasions at screen centre
    public int bulletsLeft;                     // how many bullets left at plane
    public TMP_Text bulletsQtyText;             // text showing how many bullets are left
    public TMP_Text scoreText;                  // messages shown at various occasions at screen centre
    private int score;                          // score gained by player
    private float timer = 0f;                   // used for closing of Message display
    public Slider powerSlider;                  // throttle adjuster slider of plane
    [SerializeField] Slider healthSlider;       // health of plane slider 
    [SerializeField] DynamicJoystick joy;       // plane direction controller joystick
    public AudioMixer aMixer;                   // sound mixer of game  
    public Slider volumeSlider;                 // sound volume slider in setting image
    public GameObject gameOverImage;            // Image panel opening when game is over or completed
    public GameObject needle;                   // compass needle
    public GameObject[] enemyPlane;             // the plane player fights against
    public GameObject[] enemyPosMarker;         // position of enemy on screen
    public GameObject exploCanvas;              // canvas instantiates this explosion prefab when a plane hit ground etc.
    public int ObjectivesLeft;                  // how many enemy targets to destroy to complete level
    public TMP_Text GameOverText;               // message on game over image when game ends by loss or win
    public string levelBeginString;             // the message text shown when level is loaded
    public GameObject nextStageBtn;             // button that loads next stage in build settings list
    
    private void Awake()
    {
        gearOpen = true;
        gearButtonText.text = "Close LG";       
    }

  
    private void Start()
    {
        nextStageBtn.SetActive(false);
        gameOverImage.SetActive(false);
        StartFadeOut();
        
        gameManager= GetComponent<GameManager01>();
        player = GameObject.FindWithTag("Player");
        playerPlane = player.GetComponent<PlayerPlaneScript>();
        virtCamSwitcher = player.GetComponent<VirtualCamSwitcher>();
        AdjPlanePower();
        messageText.enabled = false;
        MessageDisplay(levelBeginString, 3f);
        score = 0;
        AdjScoreText(0);
        BulletAdjust();      
        
        RefreshMarkers();

        float vol = PlayerPrefs.GetFloat("Vol", 0f);
        aMixer.SetFloat("MasterVolume", vol);
        volumeSlider.value = GetMasterLevel();
    }
  
    private void Update()
    {       
        TurnCompass();                   
        if (timer > 0)           // used for display message script
        {
            ChkTimer();
        }
        playerPlane.AdjDirection(joy.Horizontal, joy.Vertical);     // send data from joystick to plane
        if (Input.GetButtonUp("Jump"))
        {
            GearButtonPress();
        }
        if (Input.GetButtonUp("Camera"))
        {
            ChangeCamAngle();
        }
        UpdateMarkers();
    }
    private IEnumerator FadeOut()
    {
        
        gameOverImage.SetActive(true);
        GameOverText.text = "";
        yield return new WaitForSeconds(0.5f);
        float elapsedTime = 0.0f;
        Color originalColor = gameOverImage.GetComponent<Image>().color;

        while (elapsedTime < fadeTime)
        {
            yield return null; // Wait for the next frame
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            gameOverImage.GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
        gameOverImage.SetActive(false);
       
    }
    private IEnumerator FadeIn()
    {
        gameOverImage.SetActive(true);       

        float elapsedTime = 0.0f;
        Color originalColor = gameOverImage.GetComponent<Image>().color;

        while (elapsedTime <= fadeTime)
        {
            yield return null; // Wait for the next frame
            elapsedTime += Time.deltaTime;
            float alpha =  Mathf.Clamp01(elapsedTime / fadeTime)+0.01f;
            if (alpha > 1f) { alpha = 1f; }
            gameOverImage.GetComponent<Image>().color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        }
      
    }


    // Call this method to start the fade-out process
    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void ChangeCamAngle()
    {
        virtCamSwitcher.ChangeCam();
    }
    public void ObjectiveCompleted()
    {
        gameManager.SpawnEnemy0();
        ObjectivesLeft--;

        if (ObjectivesLeft < 1)
        {
            nextStageBtn.SetActive(true);       // a button to load next stage
            GameOverMessage("Mission Completed", 10f);
        }         
       Invoke("RefreshMarkers",2f) ;  
    }
    public void RefreshMarkers()            // turn on and off markers
    {
        enemyPlane = GameObject.FindGameObjectsWithTag("EnemyPlane");
        for (int i = enemyPlane.Length; i < enemyPosMarker.Length; i++)
        {
            enemyPosMarker[i].SetActive(false);
        }
    }
    private void UpdateMarkers()              // adjust red marker position
    {
        for (int i = 0; i < enemyPlane.Length; i++)
        {
            if (!enemyPlane[i])
            {
                RefreshMarkers();
                return;
            }
            if (  enemyPlane[i].activeInHierarchy)
            {
                enemyPosMarker[i].SetActive(true);
                enemyPosMarker[i].transform.position = WorldToUISpace(gameObject.GetComponent<Canvas>(), enemyPlane[i].transform.position);
            }
            else
            {
                enemyPosMarker[i].SetActive(false);
            }
        }
    }
    public Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)        // this adjust the red marker positions on screen
    {
        // Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;
        // Convert the screen point to UI rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);


        // Convert the local point to world point
        movePos.y = Mathf.Clamp(movePos.y, Screen.height * -0.53f, Screen.height * 0.45f);
        movePos.x = Mathf.Clamp(movePos.x, Screen.width * -0.49f, Screen.width * 0.49f);


        if (Vector3.Dot(Camera.main.transform.forward, (worldPos - Camera.main.transform.position)) < 0f)       // if enemy is behind camera, 
        {

            if (movePos.x > 0f)
            {
                movePos.x = Screen.width * 0.49f;           // show marker on side edge
            }
            else
            {
                movePos.x = Screen.width * -0.49f;
            }
        }
        return parentCanvas.transform.TransformPoint(movePos);
    }
    public float GetMasterLevel()           // audio master level adjustment setup. weird unity specific syntax
    {
        float value;
        bool result = aMixer.GetFloat("MasterVolume", out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }
    public void GiveExplosion(Vector3 place)
    {
        Instantiate(exploCanvas, place, Quaternion.identity);
    }


    public void MessageDisplay(string s, float t)
    {     
            timer = t;
            messageText.text = s;
            messageText.enabled = true;
    }

    public void BulletAdjust()          // bullet qty on bullet button ui button
    {
        bulletsQtyText.text = "" + bulletsLeft;
        if (bulletsLeft <= 2)
        {
            GameOverMessage("Out of Ammo", 10f);
        }
    }



    public void TurnCompass()           // adjust compass rotation
    {
        Vector3 direction1 = Quaternion.Euler(0, player.transform.eulerAngles.y, 0) * Vector3.forward;
        needle.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction1.x, direction1.z) * Mathf.Rad2Deg);
    }
      
    public void GameOverMessage(string s,float t)       // when game is lost or won
    {
        int xp = PlayerPrefs.GetInt("XP", 0);
        xp += score;
        PlayerPrefs.SetInt("XP",xp);
        int hs = PlayerPrefs.GetInt("HiScore", 0);
        if (score>hs)
        {
            PlayerPrefs.SetInt("HiScore", score);
            GameOverText.text= s + ".<br> New High Score " + score+ " <br> Congratulations ! ";     // this text is on gameoverimage UI Image gameobject
        }
        else
        {
            GameOverText.text = s + ".<br>Your score was " + score;
        }
        Invoke("EndGame", 4f);
        StartFadeIn();
    }
    private void EndGame()      // to finish the game in 1 sec
    {
      //  gameOverImage.SetActive(true);
        Time.timeScale = 0.01f;
    }
  
    
    public void GearButtonPress()       // open and close Landing gear of player plane
    {
        if (!playerPlane.grounded)      // no closing or opening of landing gear when still on ground
        {
            if (gearOpen)
            {
                playerPlane.CloseLandingGear();
                gearButtonText.text = "Open LG";
                gearOpen = false;
            }
            else
            {
                playerPlane.OpenLandingGear();
                gearButtonText.text = "Close LG";
                gearOpen = true;
            }
        }       
    }
    public void AdjSpeedText(string ss,string aa)
    {
        speedText.text = ss;
        altText.text = aa;
    }
    public void AdjScoreText(int sco)
    {
        score += sco;
        scoreText.text = score.ToString();
    }
    public void AdjHealthSlider(float f)
    {
        healthSlider.value = f;
    }
    public void AdjPlanePower()
    {
        playerPlane.AdjPower(powerSlider.value);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OpenMenu()
    {
        SceneManager.LoadScene(0);
    }
 
    private void ChkTimer()
    {      
            timer -= Time.deltaTime;
            if (timer <= 0 && messageText.enabled == true)
            {
                messageText.enabled = false;
            }        
    }

    public void SnapVertical()          // invert vertical movement controls of player plane
    {
        if (playerPlane.snapY == 1)
        {
            playerPlane.snapY = -1;
            PlayerPrefs.SetInt("SnapY", -1);
        }
        else
        {
            playerPlane.snapY = 1;
            PlayerPrefs.SetInt("SnapY", 1);
        }
    }
    public void MuteSound()
    {
        aMixer.SetFloat("MasterVolume", -60f);
        volumeSlider.value = -60f;
    }
    public void UnMuteSound()
    {
        aMixer.SetFloat("MasterVolume", 0f);
        volumeSlider.value = 0f;
    }
    public void QuitMyGame()
    {
        Application.Quit();
    }
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    public void SoundAdjust()
    {
        PlayerPrefs.SetFloat("Vol", volumeSlider.value);
        aMixer.SetFloat("MasterVolume", volumeSlider.value);       
    }
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }
}
