using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    // Fields 

    public float speed;
    public float jumpSpeed;
    public float fallingGravityMult;
    public float notHoldingJumpMult;
    public float terminalVelocity;

    private Rigidbody2D rb;
    private Collider2D collider;
    private Animator animator;
    private SpriteRenderer renderer;
    private bool walkingRight;
    private bool isJumping;

    void Start()
    {
        this.speed = 2.5f;
        this.jumpSpeed = 7f;
        this.fallingGravityMult = 1.6f;
        this.notHoldingJumpMult = 2f;
        this.terminalVelocity = 5f;
        this.rb = gameObject.GetComponent<Rigidbody2D>();
        this.collider = gameObject.GetComponent<Collider2D>();
        this.animator = gameObject.GetComponent<Animator>();
        this.renderer = gameObject.GetComponent<SpriteRenderer>();
        this.isJumping = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {

        float horMovement = Input.GetAxis("Horizontal") * speed;
        if(horMovement < 0f)
        {
            renderer.flipX = true;
        }
        else if(horMovement > 0f)
        {
            renderer.flipX = false;
        }

        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            animator.SetBool("Running", true);
        }
        else
        {
            animator.SetBool("Running", false);
        }
        animator.SetFloat("Speed", Mathf.Abs(horMovement));

        float verMovement = rb.velocity.y;

        if (!isJumping && Input.GetKey(KeyCode.LeftShift))
        {
            horMovement = horMovement * 1.75f;
        }


        if (!isJumping && Input.GetKey(KeyCode.Space))
        {
            isJumping = true;
            verMovement = jumpSpeed;
        }

        if(isJumping)
        {
            HandleJump();
        }

        /*
        if (isJumping && rb.velocity.y < 0f)
        {
            verMovement = rb.velocity.y + (Physics2D.gravity.y * (fallingGravityMult - 1) * Time.fixedDeltaTime);
        }

        
        if (isJumping && rb.velocity.y > 0f && !Input.GetKey(KeyCode.Space))
        {
            verMovement = rb.velocity.y  + (Physics2D.gravity.y * (notHoldingJumpMult - 1) * Time.fixedDeltaTime);
        }

        if (isJumping)
        {
            CheckIfCurrentlyJumping();
        }
        */

        animator.SetBool("Jumping", isJumping);
        animator.SetBool("Falling", isJumping && (rb.velocity.y < 0));

        rb.velocity = new Vector2(horMovement, verMovement);
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
            Vector2 midBottom = new Vector2(collider.bounds.center.x, collider.bounds.min.y - 0.1f);
            RaycastHit2D rayHit = Physics2D.Raycast(midBottom, Vector2.down);
            if (rayHit != null && rayHit.distance < 0.1f)
            {
                isJumping = false;
            }
        }
    }

    private float Limit(float speed)
    {
        return Mathf.Clamp(speed, -terminalVelocity, terminalVelocity);
    }
}