using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    private bool flipped;
    private Light torch;
    private SpriteRenderer rendererComp;

    void Awake()
    {
        rendererComp = GetComponent<SpriteRenderer>();
        torch = GetComponentInChildren<Light>();
        flipped = rendererComp.flipX;
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(flipped != rendererComp.flipX)
        {
            Vector3 torchPos = torch.gameObject.transform.localPosition;
            torch.gameObject.transform.localPosition = new Vector3(-torchPos.x, -torchPos.y, torchPos.z);
            flipped = rendererComp.flipX;
        }
	}
}
