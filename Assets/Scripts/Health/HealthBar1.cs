﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar1 : MonoBehaviour
{

    public Slider healthBar;
    public Health playerHealth;

    // Start is called before the first frame update
    void Start()
    {

        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;



    }


    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    }

   
}
