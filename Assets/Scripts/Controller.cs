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
    private UnityEvent OnHit;
    [SerializeField]
    private UnityEvent OnDeath;

    public float speed;
    public float jumpSpeed;
    public float fallingGravityMult;
    public float notHoldingJumpMult;
    public byte health;

    private Rigidbody2D rb;
    private Collider2D colliderComp;
    private Animator animator;
    private SpriteRenderer rendererComp;
    private bool isInMidair;
    private bool cantBeHit;
    private bool movementLocked;
    private Action onLand;

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
        this.isInMidair = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for falling out of game.
        if(transform.position.y < -1000f)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if(!movementLocked)
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

            if (!isInMidair && Input.GetKey(KeyCode.Space))
            {
                isInMidair = true;
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }

            if (isInMidair)
            {
                HandleJump();
            }

            animator.SetBool(AnimationConstants.PLAYER_JUMP, isInMidair);
            animator.SetBool(AnimationConstants.PLAYER_FALL, rb.velocity.y < -0.01f);

            // Coupled to attack system! Add lock movement api!
            if (animator.GetBool(AnimationConstants.PLAYER_AREA_ATTACK))
            {
                rb.velocity = Vector2.zero;
            }
        }

        if(cantBeHit && rb.velocity.y < -0.05f && CheckIfLanded())
        {
            if(onLand != null)
            {
                onLand.Invoke();
            }
        }

        if(rb.velocity.y < -0.1f)
        {
            isInMidair = true;
            animator.SetBool(AnimationConstants.PLAYER_FALL, true);
        }
    }

    public void Hit(GameObject enemy)
    {
        if(!cantBeHit)
        {
            Physics2D.IgnoreCollision(colliderComp, enemy.GetComponent<Collider2D>(), true);
            animator.SetBool("Hit", true);
            movementLocked = true;
            cantBeHit = true;

            int xDirection = (enemy.transform.position.x - transform.position.x) > 0f ? -1 : 1;
            rb.velocity = (new Vector2(xDirection * 4f, 8f));

            health--;
            if (health == 0)
            {
                Die();
            }

            if (OnHit != null)
            {
                OnHit.Invoke();
            }

            StartCoroutine(UnlockMovementTimeout());

            onLand = () =>
            {
                Physics2D.IgnoreCollision(colliderComp, enemy.GetComponent<Collider2D>(), false);
                animator.SetBool("Hit", false);
                movementLocked = false;
                StartCoroutine(DelayNextHit(0.5f));
                this.onLand = null;
            };
        }
    }

    public void Bounce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * 3);
        isInMidair = true;
    }

    public void Die()
    {
        // Lights go out etc.
        if(OnDeath != null)
        {
            OnDeath.Invoke();
        }
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

        if(CheckIfLanded())
        {
            isInMidair = false;
            animator.SetBool(AnimationConstants.PLAYER_JUMP, false);
            animator.SetBool(AnimationConstants.PLAYER_FALL, false);
        }
    }

    private bool CheckIfLanded()
    {
        Vector2 bottomLeft = colliderComp.bounds.min + new Vector3(0.01f, 0f, 0f);
        Vector2 bottomRight = new Vector2(bottomLeft.x + colliderComp.bounds.size.x, bottomLeft.y - 0.1f);
        Collider2D overlap = Physics2D.OverlapArea(bottomLeft, bottomRight);
        if (overlap != null && overlap != colliderComp)
        {
            return true;
        }
       
        return false;
    }
           
    private float Limit(float speed)
    {
        float terminalVelocity = 5.0f;
        return Mathf.Clamp(speed, -terminalVelocity, terminalVelocity);
    }

    public bool IsJumping()
    {
        return isInMidair;
    }

    private IEnumerator DelayNextHit(float seconds)
    {
        cantBeHit = true;
        yield return new WaitForSeconds(seconds);
        cantBeHit = false;
    }

    private IEnumerator UnlockMovementTimeout()
    {
        yield return new WaitForSeconds(3f);
        movementLocked = false;
    }
}