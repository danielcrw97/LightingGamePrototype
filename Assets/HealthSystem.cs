using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour {
    [SerializeField]
    private Sprite[] heart;
    public Image HUI;

    public int player_health = 3;
    public int damage = 1;

    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Damage();
            HUI.sprite = heart[player_health]; 
        }
    }
    public void Damage()
    {
            player_health -= damage;
    }
}
