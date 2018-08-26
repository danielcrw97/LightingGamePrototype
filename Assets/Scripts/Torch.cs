using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    private bool flipped;
    private Light torch;
    private SpriteRenderer rendererComp;
    public float energyRemaining;

    void Awake()
    {
        rendererComp = GetComponent<SpriteRenderer>();
        torch = GetComponentInChildren<Light>();
        flipped = rendererComp.flipX;
        energyRemaining = 1000f;
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

        energyRemaining = energyRemaining - Time.deltaTime;
	}

    public void SetPosition(Vector2 localPos)
    {
        torch.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, torch.gameObject.transform.localPosition.z);
    }
}
