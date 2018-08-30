using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            playaudio();
        }
        else if (para == "SettingBack")
        {
            animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, false);
            playaudio();
        }
      
        //Level Selection
        else if (para == "LevelClick")
        {
            animator.SetBool(AnimationConstants.LEVEL_SELECTION, true);
            playaudio();
        }
        else if(para == "LevelBack")
        {
            animator.SetBool(AnimationConstants.LEVEL_SELECTION, false);
            playaudio();
        }

        //Application Quit warning
        else if (para == "Quit")
        {
            animator.SetBool(AnimationConstants.APPLICATION_WARN, true);
            playaudio();
        }
        //Responce yes or no
        else if (para == "yes")
        {
            Application.Quit();
            playaudio();
        }
        else if (para == "no")
        {
            animator.SetBool(AnimationConstants.APPLICATION_WARN, false);
            playaudio();
        }

    }

    private void playaudio()
    {
        audiosource.Play();
    }
}


