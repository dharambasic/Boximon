using UnityEngine;
using System.Collections;


public class ExplosionScript : MonoBehaviour
{

	[Header("Customizable Options")]

	public float despawnTime = 10.0f;

	public float lightDuration = 0.02f;
	[Header("Light")]
	public Light lightFlash;

	[Header("Audio")]
	public AudioClip[] explosionSounds;
	public AudioSource audioSource;

	private void Start()
	{
		
		StartCoroutine(DestroyTimer());
		StartCoroutine(LightFlash());

		//Dohvača nasumićni zvuk eksplozije
		audioSource.clip = explosionSounds
			[Random.Range(0, explosionSounds.Length)];
		
		audioSource.Play();
	}

	private IEnumerator LightFlash()
	{
		//omogući svijetlost
		lightFlash.GetComponent<Light>().enabled = true;
		//Čekanje određenog vremena
		yield return new WaitForSeconds(lightDuration);
		//ugasi svijetlost
		lightFlash.GetComponent<Light>().enabled = false;
	}

	private IEnumerator DestroyTimer()
	{
		//Uništenje prefaba eksplozije nakon određenog vremena
		yield return new WaitForSeconds(despawnTime);
		Destroy(gameObject);
	}
	
}
