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

    private ParticleSystem particles;
    private Animator animator;
    private Light pointLight;
    private Light spotLight;
    private SpriteRenderer rendererComp;
    private List<Transform> crystalPositions;
    private Controller playerController;
    public TorchState state;
    public float energyRemaining;

    // Torch constants.
    public const float rechargeRate = 200f;
    public const float MAX_ENERGY = 200f;
    public const float DEFAULT_LIGHT_RANGE = 10f;
    public const float MIN_LIGHT_RANGE = 2.5f;
    private const float AREA_ATTACK_TORCH_STRENGTH = 50f;
    private const float AREA_ATTACK_TORCH_RANGE = 7f;
    private const float AREA_ATTACK_LIGHT_USAGE = 10f;
    private const float CONE_ATTACK_TORCH_RANGE = 10f;
    private const float CONE_ATTACK_LIGHT_USAGE = 3f;
    private const String POINT_LIGHT_OBJ_NAME = "TorchPointLight";
    private const String SPOT_LIGHT_OBJ_NAME = "TorchSpotLight";
    private static readonly Vector3 INIT_TORCH_POS = new Vector3(0.29f, 0.2f, -1f);
    private static readonly Vector3 AREA_ATTACK_TORCH_POS = new Vector3(-0.02f, 0.405f, -4f);
    private static readonly Vector3 SPOTLIGHT_INIT_POS = new Vector3(-0.32f, 0.139f, -0.16f);
    private static readonly Vector3 SPOTLIGHT_DOWN_POS = new Vector3(-0.038f, 0.308f, -0.16f);
    private static readonly Vector3 SPOTLIGHT_UP_POS = new Vector3(0.52f, -0.8f, -0.16f);

    //TODO FIX PARTICLE SYSTEM

    void Awake()
    {
        this.pointLight = transform.Find(POINT_LIGHT_OBJ_NAME).GetComponent<Light>();
        this.spotLight = transform.Find(SPOT_LIGHT_OBJ_NAME).GetComponent<Light>();
        pointLight.gameObject.transform.localPosition = INIT_TORCH_POS;
        spotLight.gameObject.transform.localPosition = SPOTLIGHT_INIT_POS;
        this.pointLight.range = DEFAULT_LIGHT_RANGE;

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
        this.particles = GetComponentInChildren<ParticleSystem>();
        particles.Stop();
    }
	
	// Update is called once per frame
	void Update () {

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

        if (energyRemaining <= 0f)
        {
            pointLight.enabled = false;
            spotLight.enabled = false;
            gameObject.SendMessage("Die", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void UpdateState()
    {
    }

    private void NormalTorch()
    {
        // Transition to Area
        if (Input.GetKey(KeyCode.E) && (!playerController.IsJumping()))
        {
            state = TorchState.Area;
            AreaTorch();
            return;
        }

        // Transition to Cone
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            spotLight.enabled = true;
            pointLight.enabled = false;
            state = TorchState.Cone;
            ConeTorch();
            return;
        }

        if (rendererComp.flipX)
        {
            pointLight.gameObject.transform.localPosition = new Vector3(-INIT_TORCH_POS.x, INIT_TORCH_POS.y, INIT_TORCH_POS.z);
        }
        else
        {
            pointLight.gameObject.transform.localPosition = INIT_TORCH_POS;
        }

        // Check whether the player can recharge.
        bool nextToCrystal = false;
        foreach (Transform crystalPos in crystalPositions)
        {
            // If close enough to a crystal listen for recharging inputs. 
            if ((crystalPos.position - transform.position).magnitude < 1f)
            {
                nextToCrystal = true;
                if(Input.GetKey(KeyCode.R))
                {
                    Recharge();
                    if (!particles.isPlaying)
                    {
                        particles.Play();
                    }
                }
            }
        }

        if (particles.isPlaying && (!Input.GetKey(KeyCode.R) || !nextToCrystal))
        {
            particles.Stop();
        }

        UpdateLight();

        energyRemaining = energyRemaining - Time.deltaTime;
    }

    private void AreaTorch()
    {
        // Transition to cone.
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            spotLight.enabled = true;
            pointLight.enabled = false;
            state = TorchState.Cone;
            ConeTorch();
            return;
        }

        if (!Input.GetKey(KeyCode.E))
        {
            animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
            state = TorchState.Normal;
            NormalTorch();
            return;
        }

        if (!animator.GetBool(AnimationConstants.PLAYER_AREA_ATTACK))
        {
            animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, true);
            pointLight.transform.localPosition = AREA_ATTACK_TORCH_POS;
            pointLight.range = AREA_ATTACK_TORCH_STRENGTH;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(pointLight.transform.position, AREA_ATTACK_TORCH_RANGE);
        foreach(Collider2D hit in hits)
        {
            if(hit.gameObject.tag == Tags.ENEMY_TAG)
            {
                hit.gameObject.SendMessage("HitByLight", (Vector2) pointLight.transform.position, SendMessageOptions.DontRequireReceiver);
            }
        }

        energyRemaining -= (AREA_ATTACK_LIGHT_USAGE * Time.deltaTime);
    }

    private void ConeTorch()
    {
        KeyCode input = InputUtils.CheckForMultipleInputs(KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow);
        if (input != KeyCode.None)
        {
            if(!spotLight.enabled)
            {
                spotLight.enabled = true;
            }

            ConeDirection direction = ConeDirection.UP;
            switch (input)
            {
                case KeyCode.RightArrow:
                    rendererComp.flipX = false;
                    spotLight.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                    spotLight.transform.localPosition = SPOTLIGHT_INIT_POS;
                    animator.SetBool(AnimationConstants.PLAYER_CONE_ATTACK, true);
                    animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
                    animator.SetBool("DownCone", false);
                    direction = ConeDirection.RIGHT;
                    break;

                case KeyCode.LeftArrow:
                    rendererComp.flipX = true;
                    spotLight.transform.eulerAngles = new Vector3(180f, 90f, 0f);
                    spotLight.transform.localPosition = new Vector3(-SPOTLIGHT_INIT_POS.x, SPOTLIGHT_INIT_POS.y, SPOTLIGHT_INIT_POS.z);
                    animator.SetBool(AnimationConstants.PLAYER_CONE_ATTACK, true);
                    animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
                    animator.SetBool("DownCone", false);
                    direction = ConeDirection.LEFT;
                    break;
                /*
                case KeyCode.UpArrow:
                    spotLight.transform.eulerAngles = new Vector3(-90f, 90f, 0f);
                    if(rendererComp.flipX)
                    {
                        spotLight.transform.localPosition = new Vector3(-SPOTLIGHT_UP_POS.x, SPOTLIGHT_UP_POS.y, SPOTLIGHT_UP_POS.z);
                    }
                    else
                    {
                        spotLight.transform.localPosition = SPOTLIGHT_UP_POS;
                    }
                    animator.SetBool(AnimationConstants.PLAYER_CONE_ATTACK, false);
                    animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, true);
                    animator.SetBool("DownCone", false);
                    direction = ConeDirection.UP;
                    break;
                 */

                case KeyCode.DownArrow:
                    spotLight.transform.eulerAngles = new Vector3(90f, 90f, 0f);
                    if (rendererComp.flipX)
                    {
                        spotLight.transform.localPosition = new Vector3(-SPOTLIGHT_DOWN_POS.x, SPOTLIGHT_DOWN_POS.y, SPOTLIGHT_DOWN_POS.z);
                    }
                    else
                    {
                        spotLight.transform.localPosition = SPOTLIGHT_DOWN_POS;
                    }
                    animator.SetBool(AnimationConstants.PLAYER_CONE_ATTACK, false);
                    animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
                    animator.SetBool("DownCone", true);
                    direction = ConeDirection.DOWN;
                    break;
            }

            int numberOfRays = 30;
            float rayAngle = spotLight.spotAngle;
            float distanceBetweenRays = rayAngle / numberOfRays;
            float angleHalved = rayAngle / 2;
            for(float angle=-angleHalved; angle<angleHalved; angle += distanceBetweenRays)
            {
                float worldAngle = angle;
                switch (direction)
                {
                    case ConeDirection.RIGHT:
                        worldAngle = angle;
                        break;

                    case ConeDirection.LEFT:
                        worldAngle = 180f + angle;
                        break;

                    case ConeDirection.UP:
                        worldAngle = -90f + angle;
                        break;

                    case ConeDirection.DOWN:
                        worldAngle = 90f + angle;
                        break;
                }
              
                Vector2 rayDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * worldAngle), Mathf.Sin(Mathf.Deg2Rad * worldAngle));
                RaycastHit2D[] hits = Physics2D.RaycastAll(spotLight.transform.position, rayDirection, CONE_ATTACK_TORCH_RANGE);
                foreach(RaycastHit2D hit in hits)
                {
                    if ((hit.collider != null) && (hit.collider.gameObject.tag == Tags.ENEMY_TAG))
                    {
                        hit.collider.gameObject.SendMessage("HitByLight", (Vector2)spotLight.transform.position, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
            energyRemaining -= (CONE_ATTACK_LIGHT_USAGE * Time.deltaTime);
        }
        else
        {
            spotLight.enabled = false;
            pointLight.enabled = true;
            animator.SetBool(AnimationConstants.PLAYER_CONE_ATTACK, false);
            animator.SetBool(AnimationConstants.PLAYER_AREA_ATTACK, false);
            animator.SetBool("DownCone", false);
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
        pointLight.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, pointLight.gameObject.transform.localPosition.z);
    }

    private void Recharge()
    {
        AddEnergy(50f * Time.deltaTime);
    }

    private void UpdateLight()
    {
        if(energyRemaining > 0f)
        {
            float difference = DEFAULT_LIGHT_RANGE - MIN_LIGHT_RANGE;
            float ratioOfEnergyLeft = energyRemaining / MAX_ENERGY;
            pointLight.range = DEFAULT_LIGHT_RANGE - ((1 - ratioOfEnergyLeft) * difference);
        }
        else
        {
            pointLight.range = 0f;
            spotLight.range = 0f;
        }
    }
}
