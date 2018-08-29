using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour {

    enum State
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
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private State state;
    private bool waiting;
    private bool jumping;
    private float estimatedAirTime;

    private const float WANDER_RANGE = 10f;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).transform;
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        rendererComp = GetComponent<SpriteRenderer>();
        colliderComp = GetComponent<Collider2D>();
        leftLimit = new Vector2(transform.position.x - WANDER_RANGE, transform.position.y);
        rightLimit = new Vector2(transform.position.x + WANDER_RANGE, transform.position.y);
        state = State.WANDERING;
        this.maxSpeed = 3f;
        this.jumpSpeed = 11f;
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

        if(rb.velocity.x > 0f)
        {
            rendererComp.flipX = true;
        }
        else
        {
            rendererComp.flipX = false;
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
            HandleJump();
        }
    }

    private void Chase()
    {
        if(!CanTargetPlayer())
        {
            state = State.WANDERING;
            Chase();
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
            HandleJump();
        }
    }

    private void Flee()
    {

    }

    private void HandleJump()
    {
        bool landed = HasLanded();
        if (landed)
        {
            rb.velocity = Vector2.zero;
            jumping = false;
            StartCoroutine(WaitUntilNextJump());
        }
    }

    private bool HasLanded()
    {
        if(rb.velocity.y < 0f)
        {
            Vector2 bottomLeft = colliderComp.bounds.min;
            Vector2 bottomRight = new Vector2(bottomLeft.x + (colliderComp.bounds.size.x), bottomLeft.y - 0.05f);
            Collider2D overlap = Physics2D.OverlapArea(bottomLeft, bottomRight);
            if(overlap != null && overlap != colliderComp)
            {
                return true;
            }
        }
        return false;
    }

    public void HitByLight(TorchTypes lightType)
    {
        state = State.FLEEING;
        // Set boolean flag and only disable it after 8 seconds.
    }

    private IEnumerator WaitUntilNextJump()
    {
        waiting = true;
        yield return new WaitForSeconds(4);
        waiting = false;
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
