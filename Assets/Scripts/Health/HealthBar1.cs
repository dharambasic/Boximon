using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar1 : MonoBehaviour
{

    public Slider healthBar;
    public Health playerHealth;

    void Start()
    {
        //pronalaženje igračeve komponente za zdravlje
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        healthBar = GetComponent<Slider>();
        //Postavljanje vrijednosti healthbara na maximum(100)
        healthBar.maxValue = playerHealth.maxHealth;
        healthBar.value = playerHealth.maxHealth;
    }

    //funkcija za postavljanje zdravlja igrača
    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    } 
}


