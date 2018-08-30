using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBar : MonoBehaviour {

    private Image lightBar;
    private Torch torch;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG);
        torch = player.GetComponent<Torch>();
    }

    // Use this for initialization
    void Start () {
        this.lightBar = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        float value = torch.GetEnergy() / Torch.MAX_ENERGY;
        lightBar.fillAmount = Mathf.Clamp(value, 0f, 1f); 
	}
}
