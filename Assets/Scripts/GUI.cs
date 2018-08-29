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

    public void Setting_Btn()
    {
        animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, true);
            playaudio();
    }
    public void Setting_Back_Btn()
    {
        animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, false);
            playaudio();
    }

    //Quite Application
    public void Quit()
    {
        animator.SetBool(AnimationConstants.APPLICATION_WARN, true);
            playaudio();
    }

    //warning YES or NO
    public void warningforYes()
    {
        Application.Quit();
            playaudio();
    }
    public void warningforNo()
    {
        animator.SetBool(AnimationConstants.APPLICATION_WARN, false);
            playaudio();
    }

    private void playaudio()
    {
        audiosource.Play();
    }
}


