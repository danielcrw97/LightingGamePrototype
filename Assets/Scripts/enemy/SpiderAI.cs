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
    private const float SPIDER_RANGE = 5f;

    void Awake() {
        this.target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void Start()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.rendererComp = GetComponent<SpriteRenderer>();
        this.state = State.PATROLLING;
        this.facingRight = rendererComp.flipX;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
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
            StartCoroutine(WaitThenTurnAround(3f, State.PATROLLING));
        }
        Move(facingRight);
        
        if(canTargetPlayer())
        {
            state = State.CHASING;
        }
    }

    private void Chase()
    {
        if(!canTargetPlayer())
        {
            state = State.PATROLLING;
            return;
        }

        float targetDistance = target.position.x - transform.position.x;
        bool targetIsRight = targetDistance >= 0f;
        if(targetIsRight == facingRight)
        {
            if((Mathf.Abs(targetDistance) < ATTACK_DISTANCE))
            {
                state = State.ATTACKING;
                return;
            }
            Move(facingRight);
        }
        else
        {
            StartCoroutine(WaitThenTurnAround(0.5f, State.CHASING));
        }
    }

    private void Attack()
    {
        if(!animator.GetBool("Attacking"))
        {
            animator.SetBool("Attacking", true);
        }
    }

    private void AttackOver()
    {
        state = State.CHASING;
        animator.SetBool("Attacking", false);
    }

    private void Immobilize()
    {

    }

    private void CheckForLedge()
    {

    }

    private bool canTargetPlayer()
    {
        // If the spider can sense the players and he is not beyond the spiders limits - go for it!
        bool canSense = (Mathf.Abs(target.transform.position.x - transform.position.x)) < SPIDER_RANGE;
        bool isBeyondLimits = (target.transform.position.x < leftBarrier.transform.position.x) ||
            (target.transform.position.x > rightBarrier.transform.position.x);
        return canSense && !isBeyondLimits;
    }

    private IEnumerator WaitThenTurnAround(float seconds, State newState)
    {
        state = State.IDLE;
        yield return new WaitForSeconds(seconds);
        rendererComp.flipX = !rendererComp.flipX;
        state = newState;
    }

    
}