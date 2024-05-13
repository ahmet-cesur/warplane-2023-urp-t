
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
    int difficulty;                             // diff preset its value is 0 ,1 , 2    
    public Button[] DiffButtons;                // buttons for selecting difficulty
    public Button InvertButton;                 // button inverting vertical plane controls
   
    private void Start()
    {
        LevelsImage.SetActive(false);
        SettingsImage.SetActive(false);
        CreditsImage.SetActive(false);
        PlaneImage.SetActive(false);
        int xp = PlayerPrefs.GetInt("XP", 0);
        xpText.text = "XP : " + xp;
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


}
