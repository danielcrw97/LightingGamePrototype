using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Audio_Controller : MonoBehaviour {

    private AudioSource audiosource;
    private float volume = 1f;
	void Start () {
        this.audiosource = gameObject.GetComponent<AudioSource>();
	}
	
	void FixedUpdate () {
        audiosource.volume = volume;
	}

    public void audio_controll(float para)
    {
        volume = para;
    }
}
