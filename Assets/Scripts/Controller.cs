using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Generic character controller for a 2d sprite that can run and jump.
 */
public class Controller : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private UnityEvent OnDeath;

    //TODO replace all magic numbers
    //TODO fix animation when falling off small ledges.

    public float speed;
    public float jumpSpeed;
    public float fallingGravityMult;
    public float notHoldingJumpMult;
    public byte health;

    private Rigidbody2D rb;
    private Collider2D colliderComp;
    private Animator animator;
    private SpriteRenderer rendererComp;
    private bool isJumping;

    void Start()
    {
        this.speed = 2.5f;
        this.jumpSpeed = 10f;
        this.fallingGravityMult = 1.6f;
        this.notHoldingJumpMult = 2.5f;
        this.health = 3;
        this.rb = gameObject.GetComponent<Rigidbody2D>();
        this.colliderComp = gameObject.GetComponent<Collider2D>();
        this.animator = gameObject.GetComponent<Animator>();
        this.rendererComp = gameObject.GetComponent<SpriteRenderer>();
        this.isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);

        if (rb.velocity.x < 0f && rendererComp.flipX == false)
        {
            rendererComp.flipX = true;
        }
        else if (rb.velocity.x > 0f && rendererComp.flipX == true)
        {
            rendererComp.flipX = false;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool(AnimationConstants.PLAYER_RUN, true);
        }
        else
        {
            animator.SetBool(AnimationConstants.PLAYER_RUN, false);
        }

        if (!isJumping && Input.GetKey(KeyCode.Space))
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        if(isJumping)
        {
            HandleJump();
        }

        animator.SetBool(AnimationConstants.PLAYER_JUMP, isJumping);
        animator.SetBool(AnimationConstants.PLAYER_FALL, rb.velocity.y < -0.01f);

        // Coupled to attack system! Add lock movement api!
        if (animator.GetBool(AnimationConstants.PLAYER_AREA_ATTACK))
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void Hit()
    {
        Debug.Log("Hit");
        health--;
        if(health == 0)
        {
            Die();
        }
    }

    public void Die()
    {
        // Lights go out etc.
        OnDeath.Invoke();
    }

    private void HandleJump()
    {
        if (rb.velocity.y < 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + (Physics2D.gravity.y * (fallingGravityMult - 1) * Time.fixedDeltaTime));
        }


        if (rb.velocity.y > 0f && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + (Physics2D.gravity.y * (notHoldingJumpMult - 1) * Time.fixedDeltaTime));
        }

        CheckIfStillJumping();
    }

    private void CheckIfStillJumping()
    {
        if (isJumping && rb.velocity.y < 0f)
        {
            Vector2 midBottom = new Vector2(colliderComp.bounds.center.x, colliderComp.bounds.min.y - 0.1f);
            RaycastHit2D rayHit = Physics2D.Raycast(midBottom, Vector2.down);
            if ((rayHit != null) && rayHit.distance < 0.01f)
            {
                isJumping = false;
                animator.SetBool(AnimationConstants.PLAYER_JUMP, false);
                animator.SetBool(AnimationConstants.PLAYER_FALL, false);
            }
        }
    }

    private float Limit(float speed)
    {
        float terminalVelocity = 5.0f;
        return Mathf.Clamp(speed, -terminalVelocity, terminalVelocity);
    }

    public bool IsJumping()
    {
        return isJumping;
    }
}