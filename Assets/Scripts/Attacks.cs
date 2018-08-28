using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    private Animator animator;
    private Rigidbody2D rb;
 
    void Start () {
        this.animator = GetComponent<Animator>();
        this.rb = GetComponent<Rigidbody2D>();
    }	


	void Update () {

    }

    private void ConeAttack()
    {

    }
}
