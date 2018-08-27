using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpiderAI : MonoBehaviour {

    [SerializeField]
    private GameObject waypoint1;

    [SerializeField]
    private GameObject waypoint2;

    public float speed;
    private Rigidbody2D rb;
    private Transform target;
    private Animator animator;


    private void Start() {
        this.animator = gameObject.GetComponent<Animator>();
        this.target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //follow player
        if (Vector2.Distance(transform.position, target.position) > 1.5f)
        {
            animator.SetBool("spiderAttacking", false);
            animator.SetBool("spiderWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        //Attack on player
        else
        {
            animator.SetBool("spiderAttacking", true);
            Debug.Log("attack on");
        }
    }

    public void Move(Vector2 targetPos)
    {
        
    }
}