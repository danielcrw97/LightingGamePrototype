using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAi : MonoBehaviour {

    public float speed;
    private int damage;
    private Transform targat;

    void Start() {
        targat = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //follow player
        if (Vector2.Distance(transform.position, targat.position) > 0.5)
        {
            transform.position = Vector2.MoveTowards(transform.position, targat.position, speed * Time.deltaTime);
        }
        //Attack on player
        else
        {
            Debug.Log("attack on");
        }
    }
}