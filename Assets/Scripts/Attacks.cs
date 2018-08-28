using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacks : MonoBehaviour {

    private Animator animator;
 
    void Start () {
        this.animator = gameObject.GetComponent<Animator>();
    }	
	// Update is called once per frame
	void FixedUpdate () {
        P_Cone_Attack();
        P_Circlular_Attack();
    }
    private void P_Cone_Attack()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            animator.SetTrigger(AnimationConstants.PLAYER_CONE_ATTACK);
        }
    }
    private void P_Circlular_Attack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger(AnimationConstants.PLAYER_CIRCLULAR_ATTACK); 
        }

    }
}
