﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    [SerializeField]
    private GameObject rechargeParticles;

    private ParticleSystem particles;
    private bool flipped;
    private Light torch;
    private SpriteRenderer rendererComp;
    private List<Transform> crystalPositions;
    public float energyRemaining;
    public float rechargeRate;

    public const float MAX_ENERGY = 300f;
    public const float DEFAULT_LIGHT_RANGE = 10f;
    public const float MIN_LIGHT_RANGE = 2.5f;

    void Awake()
    {
        this.torch = GetComponentInChildren<Light>();
        this.torch.range = DEFAULT_LIGHT_RANGE;
        List<GameObject> crystals = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.CRYSTAL_TAG));
        crystalPositions = new List<Transform>();
        foreach(GameObject crystal in crystals)
        {
            this.crystalPositions.Add(crystal.transform);
        }
    }

    void Start () {
        this.rendererComp = GetComponent<SpriteRenderer>();
        this.flipped = rendererComp.flipX;
        this.energyRemaining = MAX_ENERGY;
        this.rechargeRate = 200f;
        Instantiate(rechargeParticles, this.transform, false);
        this.particles = rechargeParticles.GetComponent<ParticleSystem>();
        particles.Stop();
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

        if(energyRemaining <= 0f)
        {
            Controller charController = GetComponent<Controller>();
            charController.Die();
        }

        
        foreach (Transform crystalPos in crystalPositions)
        {     
            // If close enough to a crystal listen for recharging inputs. 
            if((crystalPos.position - transform.position).magnitude < 1f && Input.GetKey(KeyCode.E))
            {
                Recharge();
                if(!particles.isPlaying)
                {
                    particles.Play();
                }
            }
        }
        if(particles.isPlaying && (Input.GetKey(KeyCode.E)))
        {
            particles.Stop();
        }

        UpdateLight();
	}

    public void SapEnergy(float amount)
    {
        energyRemaining = energyRemaining - amount;
    }

    public void AddEnergy(float amount)
    {
        energyRemaining = energyRemaining + amount;
        energyRemaining = Mathf.Clamp(energyRemaining, -50f, MAX_ENERGY);
    }

    public float GetEnergy()
    {
        return energyRemaining;
    }

    public void SetPosition(Vector2 localPos)
    {
        torch.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, torch.gameObject.transform.localPosition.z);
    }

    public void Recharge()
    {
        AddEnergy(200f * Time.deltaTime);
    }

    private void UpdateLight()
    {
        float difference = DEFAULT_LIGHT_RANGE - MIN_LIGHT_RANGE;
        float ratioOfEnergyLeft = energyRemaining / MAX_ENERGY;
        torch.range = DEFAULT_LIGHT_RANGE - ((1 - ratioOfEnergyLeft) * difference);
    }
}
