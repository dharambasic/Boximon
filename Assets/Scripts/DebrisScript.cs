using UnityEngine;
using System.Collections;


public class DebrisScript : MonoBehaviour {

	[Header("Audio")]
	public AudioClip[] debrisSounds;
	public AudioSource audioSource;

//provjerava se kolizija
	private void OnCollisionEnter (Collision collision) {
	
		if (collision.relativeVelocity.magnitude > 50) 
		{
			
			audioSource.clip = debrisSounds
				[Random.Range (0, debrisSounds.Length)];
			
			audioSource.Play ();
		}
	}
}
