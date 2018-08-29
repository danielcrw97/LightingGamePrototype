using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpiderAI : MonoBehaviour {

    public enum State
    {
        IDLE,
        PATROLLING,
        CHASING,
        ATTACKING,
        IMMOBILIZED,
        FLEEING
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
    public State state;

    // Flags
    private bool facingRight;
    private bool cantAttack;
    private bool waiting;

    private const float ATTACK_DISTANCE = 2f;
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

        this.cantAttack = false;
        this.waiting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0f)
        {
           
        }
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
                Idle();
                break;

            case State.FLEEING:
                Flee();
                break;

            default:
                Patrol();
                break;
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
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
        
        if(CanTargetPlayer())
        {
            state = State.CHASING;
        }
    }

    private void Chase()
    {
        if(waiting)
        {
            return;
        }
        
        if(IsPlayerBeyondLimits())
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
                if(!cantAttack)
                {
                    state = State.ATTACKING;
                }
                return;
            }
            
            Move(facingRight);
        }
        else
        {
            StartCoroutine(DelayFlipWhileChasing(0.4f));
        }
    }

    private void Attack()
    {
        if(!animator.GetBool("Attacking"))
        {
            animator.SetBool("Attacking", true);
        }
    }

    private void AttackStarted()
    {
    }

    private void AttackOver()
    {
        state = State.CHASING;
        animator.SetBool("Attacking", false);
        DelayAttack(1f);
    }

    private void Immobilize()
    {

    }

    private void Flee()
    {

    }

    private void Idle()
    {
        if(CanTargetPlayer())
        {
            state = State.CHASING;
        }
    }

    private void CheckForLedge()
    {

    }

    private void HitByLight(TorchTypes type)
    {
        if(type == TorchTypes.AREA)
        {
            state = State.FLEEING;
        }
        else
        {
            state = State.IMMOBILIZED;
        }
    }

    private bool CanTargetPlayer()
    {
        // If the spider can sense the players and he is not beyond the spiders limits - go for it!
        bool canSense = ((target.transform.position - transform.position).magnitude) < SPIDER_RANGE;
        bool isBeyondLimits = (target.transform.position.x < leftBarrier.transform.position.x) ||
            (target.transform.position.x > rightBarrier.transform.position.x);
        return canSense && !isBeyondLimits;
    }

    private bool IsPlayerBeyondLimits()
    {
        return (target.transform.position.x < leftBarrier.transform.position.x) ||
            (target.transform.position.x > rightBarrier.transform.position.x);
    }

    private Pair<Vector2, Vector2> GetPositionOfEdges()
    {
        //Physics2D.Raycast(GetComponent<Collider2D>().bounds.min)
        return new Pair<Vector2, Vector2>(new Vector2(), new Vector2());
    }

    private IEnumerator WaitThenTurnAround(float seconds)
    {
        state = State.IDLE;
        yield return new WaitForSeconds(seconds);
        rendererComp.flipX = !rendererComp.flipX;
        state = State.PATROLLING;
    }

    private IEnumerator DelayFlipWhileChasing(float seconds)
    {
        waiting = true;
        yield return new WaitForSeconds(seconds);
        rendererComp.flipX = !rendererComp.flipX;
        waiting = false;
    }

    private IEnumerator DelayAttack(float seconds)
    {
        cantAttack = true;
        yield return new WaitForSeconds(seconds);
        cantAttack = false;
    }
}