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
        IMMOBILIZED
    }

    public float speed;
    public float health;

    private SpriteRenderer rendererComp;
    private Rigidbody2D rb;
    private Collider2D colliderComp;
    private Transform target;
    private Animator animator;
    private Vector2 leftBarrier;
    private Vector2 rightBarrier;
    public State state;

    // Flags
    private bool facingRight;
    private bool cantAttack;
    private bool waiting;
    private bool bouncy;

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
        this.colliderComp = GetComponent<Collider2D>();
        this.state = State.PATROLLING;
        this.facingRight = rendererComp.flipX;

        this.leftBarrier = FindLeftBarrier();
        this.rightBarrier = FindRightBarrier();

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

            default:
                Patrol();
                break;
        }

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        
        if(rb.velocity.x < 0f && rendererComp.flipX)
        {
            rendererComp.flipX = false;
        }
        if (rb.velocity.x > 0f && !rendererComp.flipX)
        {
            rendererComp.flipX = true;
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
            distance = rightBarrier.x - transform.position.x;
        }
        else
        {
            distance = transform.position.x - leftBarrier.x;
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
            StartCoroutine(DelayFlipWhileChasing(0.3f));
        }
    }

    private void Attack()
    {
        if(!cantAttack && !animator.GetBool("Attacking"))
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
        StartCoroutine(DelayAttack(1f));
    }

    private void Immobilize()
    {
        if (!animator.GetBool("Stunned") && !animator.GetBool("Recovering"))
        {
            animator.SetBool("Stunned", true);
            StartCoroutine(WaitThenRecover());
        }
    }

    private void Idle()
    {
        if(CanTargetPlayer())
        {
            state = State.CHASING;
        }
    }

    private void OnStunned()
    {
        bouncy = true;
    }

    private void OnRecovered()
    {
        animator.SetBool("Recovering", false);
        state = State.CHASING;
    }

    private void HitByLight(TorchTypes type)
    {
        state = State.IMMOBILIZED;
    }

    private bool CanTargetPlayer()
    {
        // If the spider can sense the players and he is not beyond the spiders limits - go for it!
        bool canSense = ((target.transform.position - transform.position).magnitude) < SPIDER_RANGE;
        bool isBeyondLimits = (target.transform.position.x < leftBarrier.x) ||
            (target.transform.position.x > rightBarrier.x);
        return canSense && !isBeyondLimits;
    }

    private bool IsPlayerBeyondLimits()
    {
        return (target.transform.position.x < leftBarrier.x) ||
            (target.transform.position.x > rightBarrier.x);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        if (obj.tag == Tags.PLAYER_TAG)
        {
            if (bouncy && (collision.collider.bounds.min.y > transform.position.y))
            {
                animator.SetTrigger("Bounce");
                obj.SendMessage("Bounce", null, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Vector2 hitDirection = Vector3.Normalize(obj.transform.position - transform.position);
                obj.SendMessage("Hit", null, SendMessageOptions.DontRequireReceiver);
            }
        }
        
        else if(obj.tag == "Wall" || obj.tag == Tags.ENEMY_TAG)
        {
            rendererComp.flipX = !rendererComp.flipX;
        }
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

    private IEnumerator WaitThenRecover()
    {
        yield return new WaitForSeconds(5);
        bouncy = false;
        animator.SetBool("Stunned", false);
        animator.SetBool("Recovering", true);
    }

    private Vector2 FindLeftBarrier()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(colliderComp.bounds.center.x, (colliderComp.bounds.min.y - 0.1f)), Vector2.down);
        Vector2 barrier = new Vector2(hit.collider.bounds.min.x - 0.5f, transform.position.y);
        if(Mathf.Abs(transform.position.x - barrier.x) > 15f)
        {
            barrier.x = transform.position.x - 15f;
        }
        return barrier;
    }

    private Vector2 FindRightBarrier()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(colliderComp.bounds.center.x, (colliderComp.bounds.min.y - 0.1f)), Vector2.down);
        Vector2 barrier = new Vector2(hit.collider.bounds.max.x + 0.5f, transform.position.y);
        if (Mathf.Abs(transform.position.x - barrier.x) > 15f)
        {
            barrier.x = transform.position.x + 15f;
        }
        return barrier;
    }
}