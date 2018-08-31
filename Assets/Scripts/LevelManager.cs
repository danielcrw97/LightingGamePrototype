using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    [SerializeField]
    public float minX;

    [SerializeField]
    public float maxX;

    [SerializeField]
    public float minY;

    [SerializeField]
    public float maxY;

    GameObject player;

	// Use this for initialization
	void Awake () {
        player = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG);
	}
	
	// Update is called once per frame
	void Update () {
		if(player.transform.position.y < minY)
        {
            player.SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
        }
	}
}
