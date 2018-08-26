using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAi : MonoBehaviour {

    public float speed;
    private float damage;
    private Transform targat;
    private bool PlayerStatus;

    void Start() {
        targat = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.PlayerStatus = false;
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
        else
        {
            Debug.Log("attack on");
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            Debug.Log("player in range");
            PlayerStatus = true;
        }
    }
}