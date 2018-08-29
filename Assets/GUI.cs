using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private Slider fx, music;
    private bool quit;
    void Start()
    {
        this.animator = gameObject.GetComponent<Animator>();
    }
 
    public void Setting_Btn()
    {
        animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, true);
    }
    public void Setting_Back_Btn()
    {
        animator.SetBool(AnimationConstants.SETTING_BTN_CLICKED, false);
    }

    //Quite Application
    public void Quit()
    {
        animator.SetBool(AnimationConstants.APPLICATION_WARN, true);
    }

    //warning YES or NO
    public void warningforYes()
    {
        Application.Quit();
    }
    public void warningforNo()
    {
        animator.SetBool(AnimationConstants.APPLICATION_WARN, false);
    }
}


