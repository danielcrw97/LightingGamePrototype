using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpiderAI : MonoBehaviour {

    private enum State
    {
        IDLE,
        PATROLLING,
        CHASING,
        ATTACKING,
        IMMOBILIZED,
    }

    [SerializeField]
    private GameObject rightBarrier;

    [SerializeField]
    private GameObject leftBarrier;

    public float speed;
    public float health;

    private SpriteRenderer rendererComp;
    private Rigidbody2D rb;
    private Transform target;
    private Animator animator;
    private State state;
    private bool facingRight;

    private const float ATTACK_DISTANCE = 1.3f;
    private const float SPIDER_RANGE = 1.2f;

    void Awake() {
        this.target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.rendererComp = GetComponent<SpriteRenderer>();
        this.state = State.PATROLLING;
        this.facingRight = !rendererComp.flipX;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        this.facingRight = rendererComp.flipX;

        switch(state)
        {
            case State.PATROLLING:
                Patrol();
                break;

            case State.ATTACKING:
                Attack();
                break;

            case State.CHASING:
                Chase();
                break;

            case State.IMMOBILIZED:
                Immobilize();
                break;

            case State.IDLE:
                break;

            default:
                Patrol();
                break;
        }
        Debug.Log(rb.velocity.x);
    }

    public void Move(bool moveRight)
    {
        if (moveRight)
        {
            this.rb.velocity = Vector2.right * speed;
        }
        else
        {
            this.rb.velocity = Vector2.left * speed;
        }
    }

    private void Patrol()
    {
        float distance;
        if(facingRight)
        {
            distance = rightBarrier.transform.position.x - transform.position.x;
        }
        else
        {
            distance = transform.position.x - leftBarrier.transform.position.x;
        }
        if(distance < 0.05f)
        {
            StartCoroutine(WaitThenTurnAround(3f));
        }
        Move(facingRight);
    }

    private void Chase()
    {

    }

    private void Attack()
    {

    }

    private void Immobilize()
    {

    }

    private void CheckForLedge()
    {

    }

    public void SwitchDirection()
    {

    }

    private bool canTargetPlayer()
    {
        // If the spider can sense the players and he is not beyond the spiders limits - go for it!
        bool canSense = ((target.transform.position - transform.position).magnitude) < SPIDER_RANGE;
        bool isBeyondLimits = (target.transform.position.x < leftBarrier.transform.position.x) ||
            (target.transform.position.x > rightBarrier.transform.position.x);
        return canSense && !isBeyondLimits;
    }

    private IEnumerator WaitThenTurnAround(float seconds)
    {
        state = State.IDLE;
        yield return new WaitForSeconds(seconds);
        rendererComp.flipX = !rendererComp.flipX;
        state = State.PATROLLING;
    }
}