using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeToNextScene : MonoBehaviour {

    [SerializeField]
    private string nextScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
