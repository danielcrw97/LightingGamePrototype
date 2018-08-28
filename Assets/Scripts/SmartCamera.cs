using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour {

    private Transform target;
    private Vector3 offset;

    public float smoothSpeed = 0.125f;

    void Awake()
    {
        target = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG).transform;
        transform.position = target.position + offset;
    }
    // Use this for initialization
    void Start () {
        offset = new Vector3(0f, 0f, -10f); 
    }
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
	}
}
