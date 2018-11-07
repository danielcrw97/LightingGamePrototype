using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class WinMenu : MonoBehaviour {

    private Animator animator;
    void Start()
    {
        this.animator = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D con)
    {
        if (con.gameObject.tag == "Player")
        {
            print("h");
            animator.SetBool(AnimationConstants.APPLICATION_WARN, true);
        }

    }
}
