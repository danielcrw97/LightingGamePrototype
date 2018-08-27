using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour {

    private bool flipped;
    private Light torch;
    private SpriteRenderer rendererComp;
    private List<Transform> crystalPositions;
    public float energyRemaining;
    public float rechargeRate;

    void Awake()
    {
        this.torch = GetComponentInChildren<Light>();
        List<GameObject> crystals = new List<GameObject>(GameObject.FindGameObjectsWithTag(Tags.CRYSTAL_TAG));
        foreach(GameObject crystal in crystals)
        {
            crystalPositions.Add(crystal.transform);
        }
    }

    void Start () {
        this.rendererComp = GetComponent<SpriteRenderer>();
        this.flipped = rendererComp.flipX;
        this.energyRemaining = 1000f;
        this.rechargeRate = 200f;
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
            // Die
        }

        foreach(Transform crystalPos in crystalPositions)
        {
            // If close enough to a crystal listen for recharging inputs. 
            if((crystalPos.position - transform.position).magnitude < 1f)
            {

            }
        }
	}

    public void SapEnergy(float amount)
    {
        energyRemaining = energyRemaining - amount;
    }

    public void SetPosition(Vector2 localPos)
    {
        torch.gameObject.transform.localPosition = new Vector3(localPos.x, localPos.y, torch.gameObject.transform.localPosition.z);
    }

    public void Recharge()
    {

    }

    IEnumerator RechargeCoroutine(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, smoothing * Time.deltaTime);

            yield return null;
        }

        print("Reached the target.");

        yield return new WaitForSeconds(3f);

        print("MyCoroutine is now finished.");
    }
}
