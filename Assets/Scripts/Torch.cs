using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    [SerializeField]
    private GameObject rechargeParticles;

    private ParticleSystem particles;
    private bool flipped;
    private Animator animator;
    private Light torch;
    private SpriteRenderer rendererComp;
    private List<Transform> crystalPositions;
    public float energyRemaining;
    public float rechargeRate;

    public const float MAX_ENERGY = 300f;
    public const float DEFAULT_LIGHT_RANGE = 10f;
    public const float MIN_LIGHT_RANGE = 2.5f;

    private bool areaAttackActive;
    private bool coneAttackActive;

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
        this.animator = GetComponent<Animator>();
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
            torch.gameObject.transform.localPosition = new Vector3(-torchPos.x, torchPos.y, torchPos.z);
            flipped = rendererComp.flipX;
        }

        energyRemaining = energyRemaining - Time.deltaTime;

        if(energyRemaining <= 0f)
        {
            gameObject.SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
        }

        
        // Check whether the player can recharge.
        foreach (Transform crystalPos in crystalPositions)
        {     
            // If close enough to a crystal listen for recharging inputs. 
            if((crystalPos.position - transform.position).magnitude < 1f && Input.GetKey(KeyCode.R))
            {
                Recharge();
                if(!particles.isPlaying)
                {
                    particles.Play();
                }
            }
        }
        if(particles.isPlaying && (Input.GetKey(KeyCode.R)))
        {
            particles.Stop();
        }


        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool("ConeAttack", true);
            ConeAttack();
            coneAttackActive = true;
        }
        else if(animator.GetBool("ConeAttack"))
        {
            animator.SetBool("ConeAttack", false);
        }

        if (Input.GetKey(KeyCode.Z))
        {
            animator.SetTrigger(AnimationConstants.PLAYER_CIRCLULAR_ATTACK);
        }

        UpdateLight();
	}

    public float GetEnergy()
    {
        return energyRemaining;
    }

    private void ConeAttack()
    {

    }

    private void SapEnergy(float amount)
    {
        energyRemaining = energyRemaining - amount;
    }

    private void AddEnergy(float amount)
    {
        energyRemaining = energyRemaining + amount;
        energyRemaining = Mathf.Clamp(energyRemaining, -50f, MAX_ENERGY);
    }

    private void SetPosition(Vector2 localPos)
    {
        torch.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, torch.gameObject.transform.localPosition.z);
    }

    private void Recharge()
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
