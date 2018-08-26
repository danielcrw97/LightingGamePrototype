using System.Collections;
using UnityEngine;


public class halth_controll : MonoBehaviour
{
    public float player_halth;
    void Start()
    {
        this.player_halth = 100.0f;
    }
    private void Update()
    {
        if (player_halth <= 0)
        {
            Destroy(gameObject);
        }
    }
}