using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour {

    public enum State
    {
        WANDERING,
        CHASING,
        FLEEING
    }

    public float maxSpeed;
    public float jumpSpeed;

    private Rigidbody2D rb;
    private Collider2D colliderComp;
    private Transform target;
    private SpriteRenderer rendererComp;
    private Animator animator;
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private Vector2 lastHitByLight;
    public State state;
    private Coroutine waitingRoutine;
    private bool waiting;
    private bool jumping;
    private float estimatedAirTime;

    private const float WANDER_RANGE = 10f;
    private const float FLEE_RANGE = 10f;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).transform;
    }

    // Use this for initialization
    void Start () { 
        rb = GetComponent<Rigidbody2D>();
        rendererComp = GetComponent<SpriteRenderer>();
        colliderComp = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        leftLimit = new Vector2(transform.position.x - WANDER_RANGE, transform.position.y);
        rightLimit = new Vector2(transform.position.x + WANDER_RANGE, transform.position.y);
        state = State.WANDERING;
        lastHitByLight = new Vector2();
        this.maxSpeed = 3f;
        this.jumpSpeed = 11f;
        float random = UnityEngine.Random.Range(0f, 2f);
        WaitUntilNextJump(random);
	}

    void FixedUpdate()
    {
        if(state == State.WANDERING)
        {
            Wander();
        }
        else if(state == State.CHASING)
        {
            Chase();
        }
        else if(state == State.FLEEING)
        {
            Flee();
        }

        if (rb.velocity.x < 0f && rendererComp.flipX)
        {
            rendererComp.flipX = false;
        }
        if (rb.velocity.x > 0f && !rendererComp.flipX)
        {
            rendererComp.flipX = true;
        }

        animator.SetFloat("VerticalSpeed", rb.velocity.y);
    }

    void Update()
    {
        if(transform.position.y < -5000f)
        {
            Destroy(gameObject);
        }
    }

    private void Wander()
    {   
        if(CanTargetPlayer())
        {
            state = State.CHASING;
            Chase();
        }
        else if(!waiting && !jumping)
        {
            float randomSpeed = (maxSpeed / 2) + ((maxSpeed / 2) * UnityEngine.Random.value);
            randomSpeed = randomSpeed * RandomSign();
            if(WillFishJumpOutOfBounds(randomSpeed))
            {
                randomSpeed = -randomSpeed;
            }
            rb.velocity = new Vector2(randomSpeed, jumpSpeed);
            jumping = true;
            
        }
        else if(jumping)
        {
            HandleJump(4f);
        }
    }

    private void Chase()
    {
        if(!CanTargetPlayer())
        {
            state = State.WANDERING;
            Wander();
        }

        else if (!waiting && !jumping)
        {
            float airTime = EstimateAirTimeInSeconds();
            float distanceToPlayer = target.position.x - transform.position.x;
            float requiredSpeed = distanceToPlayer / airTime;
            float speed = Mathf.Clamp(requiredSpeed, -maxSpeed, maxSpeed);
            rb.velocity = new Vector2(speed, jumpSpeed);
            jumping = true;
        }
        
        else if(jumping)
        {
            HandleJump(3f);
        }
    }

    private void Flee()
    {
        if ((Mathf.Abs(lastHitByLight.x - transform.position.x)) > FLEE_RANGE)
        {
            state = State.WANDERING;
        }

        if (!waiting && !jumping)
        {
            if ((lastHitByLight.x - transform.position.x) > 0f)
            {
                rb.velocity = new Vector2(-maxSpeed, jumpSpeed);
            }
            else
            {
                rb.velocity = new Vector2(maxSpeed, jumpSpeed);
            }
            jumping = true;
        }
        else if (jumping)
        {
            HandleJump(0.5f);
        }
    }

    private void HandleJump(float landingWaitTime)
    {
        if (HasLanded())
        {
            jumping = false;
            waitingRoutine = StartCoroutine(WaitUntilNextJump(landingWaitTime));
        }
    }

    private bool HasLanded()
    {
        if(rb.velocity.y < 0f)
        {
            Vector2 bottomLeft = colliderComp.bounds.min - new Vector3(0f, 0.05f, 0f);
            Vector2 bottomRight = new Vector2(bottomLeft.x + (colliderComp.bounds.size.x), bottomLeft.y - 0.05f);
            Collider2D overlap = Physics2D.OverlapArea(bottomLeft, bottomRight);
            if(overlap != null && overlap != colliderComp)
            {
                return true;
            }
        }
        return false;
    }

    public void HitByLight(Vector2 lightPos)
    {
        if(waitingRoutine != null)
        {
            StopCoroutine(waitingRoutine);
        }
        waiting = false;
        lastHitByLight = lightPos;
        state = State.FLEEING;
        Flee();
    }

    private IEnumerator WaitUntilNextJump(float waitTime)
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        waiting = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;
        if(obj.tag == Tags.PLAYER_TAG)
        {
            obj.SendMessage("Hit", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        else if(obj.tag == "Wall" || obj.tag == Tags.ENEMY_TAG)
        {
            // In the unlikely event of colliding, flee from the source.
            HitByLight(obj.transform.position);
        }
    }

    private float EstimateAirTimeInSeconds()
    {
        return Mathf.Abs((jumpSpeed * 2) / Physics2D.gravity.y);
    }

    private bool CanTargetPlayer()
    {
        bool isPlayerBeyondLimits = (target.position.x < leftLimit.x) || (target.position.x > rightLimit.x);
        bool isPlayerCloseEnough = ((target.position - transform.position).magnitude) < 15f;
        return isPlayerCloseEnough && !isPlayerBeyondLimits;
    }

    private bool WillFishJumpOutOfBounds(float xVelocity)
    {
        float maxDistance = EstimateAirTimeInSeconds() * xVelocity;
        float maxPos = transform.position.x + maxDistance;
        if(maxPos < leftLimit.x || maxPos > rightLimit.x)
        {
            return true;
        }
        return false;
    }

    private int RandomSign()
    {
        return UnityEngine.Random.value < 0.5 ? 1 : -1;
    }
}
