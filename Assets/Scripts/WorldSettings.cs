using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour {

    private const float GRAV_MULTIPLIER = 1.5f;

	// Use this for initialization
	void Start () {
        Physics2D.gravity = Physics2D.gravity * GRAV_MULTIPLIER;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
