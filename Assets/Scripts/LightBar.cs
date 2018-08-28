using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightBar : MonoBehaviour {

    private Slider slider;
    private Torch torch;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(Tags.PLAYER_TAG);
        torch = player.GetComponent<Torch>();
    }

    // Use this for initialization
    void Start () {
        this.slider = GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        float energyRemaining = torch.GetEnergy();
        float value = energyRemaining / Torch.MAX_ENERGY;
        slider.value = Mathf.Clamp(value, 0f, 1f); 
	}
}
