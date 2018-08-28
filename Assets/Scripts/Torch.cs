using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    public enum TorchState
    {
        Normal,
        Area,
        Cone
    }

    enum ConeDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    [SerializeField]
    private GameObject rechargeParticles;

    private ParticleSystem particles;
    private Animator animator;
    private Light torch;
    private SpriteRenderer rendererComp;
    private List<Transform> crystalPositions;
    private Vector3 initialTorchPos;
    private Controller playerController;
    public TorchState state;
    public float energyRemaining;

    public const float rechargeRate = 200f;
    public const float MAX_ENERGY = 300f;
    public const float DEFAULT_LIGHT_RANGE = 10f;
    public const float MIN_LIGHT_RANGE = 2.5f;
    private const float AREA_ATTACK_TORCH_RANGE = 50f;
    private static Vector3 AREA_ATTACK_TORCH_POS = new Vector3(-0.02f, 0.405f, -4f);

    //TODO FIX PARTICLE SYSTEM

    void Awake()
    {
        this.torch = GetComponentInChildren<Light>();
        this.torch.range = DEFAULT_LIGHT_RANGE;
        initialTorchPos = torch.transform.localPosition;
        List<GameObject> crystals = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.CRYSTAL_TAG));
        crystalPositions = new List<Transform>();
        foreach(GameObject crystal in crystals)
        {
            this.crystalPositions.Add(crystal.transform);
        }
    }

    void Start () {
        this.playerController = GetComponent<Controller>();
        this.rendererComp = GetComponent<SpriteRenderer>();
        this.animator = GetComponent<Animator>();
        this.energyRemaining = MAX_ENERGY;
        this.state = TorchState.Normal;
        Instantiate(rechargeParticles, this.transform, false);
        this.particles = rechargeParticles.GetComponent<ParticleSystem>();
        particles.Stop();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateState();

        switch(state)
        {
            case TorchState.Normal:
                NormalTorch();
                break;

            case TorchState.Area:
                AreaTorch();
                break;

            case TorchState.Cone:
                ConeTorch();
                break;

            default:
                NormalTorch();
                break;
        }

    }

    private void UpdateState()
    {
        // Transition to Area
        if (Input.GetKey(KeyCode.E) && (!playerController.IsJumping()))
        {
            state = TorchState.Area;
            AreaTorch();
            return;
        }

        // Transition to Cone
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            state = TorchState.Cone;
            ConeTorch();
            return;
        }

        state 
    }

    private void NormalTorch()
    {

        if (rendererComp.flipX)
        {
            torch.gameObject.transform.localPosition = new Vector3(-initialTorchPos.x, initialTorchPos.y, initialTorchPos.z);
        }
        else
        {
            torch.gameObject.transform.localPosition = initialTorchPos;
        }

        if (energyRemaining <= 0f)
        {
            torch.enabled = false;
            gameObject.SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
        }


        // Check whether the player can recharge.
        foreach (Transform crystalPos in crystalPositions)
        {
            // If close enough to a crystal listen for recharging inputs. 
            if ((crystalPos.position - transform.position).magnitude < 1f && Input.GetKey(KeyCode.R))
            {
                Recharge();
                if (!particles.isPlaying)
                {
                    particles.Play();
                }
            }
        }
        if (particles.isPlaying && (Input.GetKey(KeyCode.R)))
        {
            particles.Stop();
        }

        UpdateLight();

        energyRemaining = energyRemaining - Time.deltaTime;
    }

    private void AreaTorch()
    {
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            state = TorchState.Cone;
            ConeTorch();
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
            state = TorchState.Normal;
            NormalTorch();
            return;
        }

        if (!animator.GetBool(AnimationConstants.PLAYER_AREA_ATTACK))
        {
            animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, true);
            torch.transform.localPosition = AREA_ATTACK_TORCH_POS;
            torch.range = AREA_ATTACK_TORCH_RANGE;
        }

        energyRemaining -= (3 * Time.deltaTime);
    }

    private void ConeTorch()
    {
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            if(!animator.GetBool("ConeAttack"))
            {
                animator.SetBool("ConeAttack", true);
            }
            state = TorchState.Cone;
            ConeDirection direction;
        
        }
        else
        {
            state = TorchState.Normal;
            NormalTorch();
            return;
        }
    }

    public float GetEnergy()
    {
        return energyRemaining;
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
