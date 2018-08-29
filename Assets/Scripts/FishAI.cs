using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour {

    enum State
    {
        WANDERING,
        CHASING
    }

    public float speed;

    private Rigidbody2D rb;
    private Transform target;
    private SpriteRenderer rendererComp;
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private State state;

    private const float WANDER_RANGE = 10f;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).transform;
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        rendererComp = GetComponent<SpriteRenderer>();
        leftLimit = new Vector2(transform.position.x - WANDER_RANGE, transform.position.y);
        rightLimit = new Vector2(transform.position.x + WANDER_RANGE, transform.position.y);
        state = State.WANDERING;
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

    }

    private void Chase()
    {

    }

    public void HitByLight(TorchTypes lightType)
    {
        Debug.Log("Hit");
    }

    private float EstimateAirTime()
    {
        return 0f;
    }
}
