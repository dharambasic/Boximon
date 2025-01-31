﻿using UnityEngine;
using System.Collections;


public class Grenade : MonoBehaviour {

	[Header("Timer")]
	public float grenadeTimer = 5.0f;

	[Header("Explosion Prefabs")]
	public Transform explosionPrefab;
	
	[Header("Explosion Options")]
	public float radius = 25.0F;
	public float power = 350.0F;

	[Header("Throw Force")]
	public float minimumForce = 1500.0f;
	public float maximumForce = 2500.0f;
	private float throwForce;

	[Header("Audio")]
	public AudioSource impactSound;

	private void Awake () 
	{
		//Nasumična sila bacanja
		throwForce = Random.Range
			(minimumForce, maximumForce);
		//Nasumična rotacija granate
		GetComponent<Rigidbody>().AddRelativeTorque 
		   (Random.Range(500, 1500), 
			Random.Range(0,0), 		 
			Random.Range(0,0)  		 
			* Time.deltaTime * 5000);
	}

	private void Start () 
	{
		//Bacanje granate nasumičnom silom
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * throwForce);

		//Startanje timera eksplozije
		StartCoroutine (ExplosionTimer ());
	}
	private IEnumerator ExplosionTimer () 
	{
		//Čekanje isteka timera eksplozije
		yield return new WaitForSeconds(grenadeTimer);

		//Raycast kontrolira ako je granata pala na zemlju
		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{			
			Instantiate (explosionPrefab, checkGround.point, 
				Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		//Sila eksplozije
		Vector3 explosionPos = transform.position;
		//Korištenje overlasphere funkcije za provjeru kolizije
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();

			//Dodavanje sile bližnjim neprijateljima
			if (rb != null)
				rb.AddExplosionForce (power * 5, explosionPos, radius, 3.0F);
			
			//Provjera ako je eksplozija oštetila neprijatelja
			if (hit.GetComponent<Collider>().tag == "Enemy")
			     
			{
						//oduzimanje zdravlja neprijatelja
				hit.gameObject.GetComponent<Enemy>().ApplyDamage(100);
			}		
		}
		//Uništavanje granate nakon eksplozije
		Destroy (gameObject);
	}
	}
