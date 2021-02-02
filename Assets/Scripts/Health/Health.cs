using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IEntity
{

    public int currentHealth = 0;
    public int maxHealth = 100;
    public HealthBar1 healthBar;
    public HealthBar1 healthBar1;

    //Postavljanje trenutnog zdravlja na max
    //Pozivanje funkcije regeneracije zdravlja
    void Start()
    {
        currentHealth = maxHealth;
        InvokeRepeating("Regeneration", 1f, 5f);
    }

    //provjeravanje zdravlja igrača
    void Update()
    {
        
        healthBar.SetHealth(currentHealth);
        healthBar1.SetHealth(currentHealth);
     
    }

    //Oduzimanje zdravlja od strane neprijatelja
    public void ApplyDamage (float points)
    {
        currentHealth -= (int)points;
        healthBar.SetHealth(currentHealth);
        healthBar1.SetHealth(currentHealth);
    }

    //funkcija za regeneraciju zdravlja
    public void Regeneration()
    {
        if (currentHealth < 100) {
            currentHealth += 5;
            healthBar.SetHealth(currentHealth);
            healthBar1.SetHealth(currentHealth);
           }
    }

    
}
