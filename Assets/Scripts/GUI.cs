using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUI : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private Slider fx, music;
    private AudioSource audiosource;

    void Start()
    {
        this.animator = gameObject.GetComponent<Animator>();
        this.audiosource = gameObject.GetComponent<AudioSource>();
    }

    public void UI(string para) 
    {
        //Setting Menu
        if (para == "SettingClick")
        {
            animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, true);
        }
        else if (para == "SettingBack")
        {
            animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, false);
        }
      
        //Level Selection
        else if (para == "LevelClick")
        {
            animator.SetBool(AnimationConstants.LEVEL_SELECTION, true);
        }
        else if(para == "LevelBack")
        {
            animator.SetBool(AnimationConstants.LEVEL_SELECTION, false);
        }
        else if (para == "AboutOpen")
        {
            animator.SetBool("About_Open", true);
        }
        else if (para == "AboutClose")
        {
            animator.SetBool("About_Open", false);
        }


        //Application Quit warning
        else if (para == "Quit")
        {
            animator.SetBool(AnimationConstants.APPLICATION_WARN, true);
        }
        //Responce yes or no
        else if (para == "yes")
        {
            Application.Quit();
        }
        else if (para == "no")
        {
            animator.SetBool(AnimationConstants.APPLICATION_WARN, false);
        }
        playaudio();
    }

    public void Levels(int para)
    {
        if (para == 1)
        {
            SceneManager.LoadScene("Level 1");
            //level 1
        }
        else if (para == 2)
        {
            SceneManager.LoadScene("Level 2");
            //level 2
        }
        else if (para == 3)
        {
            SceneManager.LoadScene("Level 3");
            //level 3
        }
        else if (para == 4)
        {
            SceneManager.LoadScene("Level 4");
            //level 4
        }
        else if (para == 5)
        {
            SceneManager.LoadScene("Level 5");
            //level 4
        }
        else if (para == 6)
        {
            SceneManager.LoadScene("Level 6");
            //level 4
        }
        playaudio();
    }
    private void playaudio()
    {
        audiosource.Play();
    }
}


