using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTrigger : MonoBehaviour {

    private Animator fader;

    void Awake()
    {
        fader = GameObject.Find("BlackFade").GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == Tags.PLAYER_TAG)
        {
            fader.SetTrigger("FadeOut");
        }
    }
}
