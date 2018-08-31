using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGameOver : MonoBehaviour {

    [SerializeField]
    private GameObject textComp;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameOver = GameObject.Find("GameOver");
        gameOver.SetActive(true);
        gameOver.GetComponent<Text>().text = "Game Over";
    }

    private IEnumerator WaitThenEndGame()
    {
        yield return new WaitForSeconds(5);
        Application.Quit();
    }
}
