using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyAi : MonoBehaviour {

    public float speed;
    private float damage;
    private Transform targat;
    private bool PlayerStatus;
    private halth_controll HalthControll;
    public Slider HalthBar;

    private void Start() {
        targat = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        HalthControll= GameObject.FindGameObjectWithTag("Player").GetComponent<halth_controll>();
        this.PlayerStatus = false;
        HalthBar.value = HalthControll.player_halth;
    }
    // Update is called once per frame
    void Update()
    {
       
        //follow player
        if (Vector2.Distance(transform.position, targat.position) > 1 && PlayerStatus == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, targat.position, speed * Time.deltaTime);
        }
        //Attack on player
        else if(PlayerStatus == true)
        {
            Debug.Log("attack on");
            HalthControll.player_halth -= 0.2f;
        }
        HalthBar.value = HalthControll.player_halth;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            Debug.Log("player in range");
            PlayerStatus = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Debug.Log("player out off range");
        PlayerStatus = false;
    }
}