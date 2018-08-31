using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathFade : MonoBehaviour {

    private Animator fade;

	// Use this for initialization
	void Start () {
        fade = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DeathListener()
    {
        fade.SetTrigger("Death");
    }

    void OnFade()
    {
        // Restart level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
