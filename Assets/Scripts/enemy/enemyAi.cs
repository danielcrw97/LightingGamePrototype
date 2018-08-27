using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class enemyAi : MonoBehaviour {


    public float speed;
    private float damage;
    private Transform targat;
    private bool PlayerStatus;
    private Animator animator;


    private void Start() {
        this.animator = gameObject.GetComponent<Animator>();
        targat = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        this.PlayerStatus = false;
        animator.SetBool("spiderAttcking", false);


    }
    // Update is called once per frame
    void Update()
    {
       
        //follow player
        if (Vector2.Distance(transform.position, targat.position) > 1 && PlayerStatus == true)
        {
            animator.SetBool("spiderAttacking", false);
            animator.SetBool("spiderWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, targat.position, speed * Time.deltaTime);
        }
        //Attack on player
        else if(PlayerStatus == true)
        {
            animator.SetBool("spiderAttacking", true);
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

    private void OnTriggerExit2D(Collider2D col)
    {
        animator.SetBool("spiderWalking", false);
        Debug.Log("player out off range ");
        PlayerStatus = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "wall")
        {
            Debug.Log("enemy try to jump small walls else retuen to patroling state");
        }        
    }
}