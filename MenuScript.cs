
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public GameObject LevelsImage;
    public GameObject SettingsImage;
    public GameObject CreditsImage;
    public GameObject PlaneImage;
    public AudioMixer aMixer;                   // sound mixer of game  
    public Slider volumeSlider;                 // slider adjusting sound volume
    private int hiscore;                        // high score
    public TMP_Text hiScoreText;                // text box of high score
    public TMP_Text xpText;                     // text box of XP - experience points
    public TMP_Text rankText;                   // what is pilots rank. calc by XP
    public TMP_Text goldstext;                  // collected golds text
    int difficulty;                             // diff preset its value is 0 ,1, 2    
    public Button[] DiffButtons;                // buttons for selecting difficulty
    public Button InvertButton;                 // button inverting vertical plane controls
    string[] ranks;                      // pilot ranks in air force

    private void Start()
    {
        ranks = new string[] { "Second Lieutenant", "First Lieutenant", "Captain", "Major", "Lieutenant Colonel", "Colonel", "Brigadier General", "Major General", "Lieutenant General", "General", "General of the Air Force" };
        LevelsImage.SetActive(false);
        SettingsImage.SetActive(false);
        CreditsImage.SetActive(false);
        PlaneImage.SetActive(false);
        int xp = PlayerPrefs.GetInt("XP", 0);
        xpText.text = "XP : " + xp;
        AdjustRanks(xp);
        hiscore = PlayerPrefs.GetInt("HiScore", 0);
        hiScoreText.text= "High Score : "+ hiscore;
        volumeSlider.value = GetMasterLevel();   
        difficulty = PlayerPrefs.GetInt("Diff", 0);
        AdjustButtonColors(difficulty);
        
        int val = PlayerPrefs.GetInt("SnapY", 1);
        if (val == 1)
        {            
            InvertButton.image.color = Color.white;
        }
        else
        {           
            InvertButton.image.color = Color.gray;
        }

    }

    public void SelectPlaneType(int i)
    {
        PlayerPrefs.SetInt("Type", i);
    }

   
    private void AdjustButtonColors(int d)
    {
        for(int i = 0; i < DiffButtons.Length; i++) 
        {
            if(i!=d)
            {
                DiffButtons[i].image.color = Color.white;
            }
            
        }
        DiffButtons[d].image.color = Color.gray;
    }

    public void AdjustDifficulty(int d)
    {
        difficulty = d;
        PlayerPrefs.SetInt("Diff", d);
        AdjustButtonColors(d);
    }

    public float GetMasterLevel()           // adjust master sound volume at player prefs
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
    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
    public void QuitApp()
    {
        Application.Quit();
    }
    public void MuteSound()
    {
        aMixer.SetFloat("MasterVolume", -60f);
        volumeSlider.value = -60f;
    }

    public void SoundAdjust()
    {
        aMixer.SetFloat("MasterVolume",volumeSlider.value );       
    }

    public void SnapVertical()
    {
        int val = PlayerPrefs.GetInt("SnapY", 1);
        if (val == 1)
        {          
            PlayerPrefs.SetInt("SnapY", -1);
            InvertButton.image.color = Color.gray;
        }
        else
        {            
            PlayerPrefs.SetInt("SnapY", 1);      
            InvertButton.image.color = Color.white;
        }
    }
    private void AdjustRanks(int x)
    {
        rankText.text = "Your rank : <br>";
        int r = x / 100;
        switch (r)
        {
            case 0: rankText.text += ranks[0]; break;
            case 1: rankText.text = ranks[1]; break;
            case 2: rankText.text = ranks[2]; break;
            case 3: rankText.text = ranks[3]; break;
            case 4: rankText.text = ranks[4]; break;
            case 5: rankText.text = ranks[5]; break;
            case 6: rankText.text = ranks[6]; break;
            case 7: rankText.text = ranks[7]; break;
            case 8: rankText.text = ranks[8]; break;
            case 9: rankText.text = ranks[9]; break;
            case 10: rankText.text = ranks[10]; break;
            default: rankText.text = ranks[11]; break;


        }

       
      
    }

}
