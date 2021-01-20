using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knife : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

            if (collision.gameObject.CompareTag("Enemy")) {

			collision.gameObject.GetComponent<SC_NPCEnemy>().ApplyDamage(100);

            }
    }
}
