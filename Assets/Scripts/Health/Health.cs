using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IEntity
{

    public int currentHealth = 0;
    public int maxHealth = 100;
    

    public HealthBar1 healthBar;
    public HealthBar1 healthBar1;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        InvokeRepeating("Regeneration", 1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealth(currentHealth);
        healthBar1.SetHealth(currentHealth);
     
    }


    public void ApplyDamage (float points)
    {
        currentHealth -= (int)points;

       

        healthBar.SetHealth(currentHealth);
        healthBar1.SetHealth(currentHealth);
    }

    public void Regeneration()
    {

        if (currentHealth < 100) {
            currentHealth += 5;
            healthBar.SetHealth(currentHealth);
            healthBar1.SetHealth(currentHealth);
           }
    }

    
}
