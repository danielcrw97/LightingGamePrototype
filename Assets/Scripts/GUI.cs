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
             //level 1
           }
             else if (para == 2)
                {
                  //level 2
                }
                 else if (para == 3)
                   {
                     //level 3
                   }
                    else if (para == 4)
                       {
                        //level 4
                       }
    playaudio();
    }
    private void playaudio()
    {
        audiosource.Play();
    }
}


