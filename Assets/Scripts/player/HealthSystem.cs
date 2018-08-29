using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : Controller{
    [SerializeField]
    private Sprite[] heart;
    public Image HUI;


    void FixedUpdate() {
            HUI.sprite = heart[health];
    }
}
