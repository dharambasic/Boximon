using UnityEngine;
using System.Collections;

public class Casing : MonoBehaviour {

	[Header("Force X")]
	[Tooltip("Minimum force on X axis")]
	public float minimumXForce;		
	[Tooltip("Maimum force on X axis")]
	public float maximumXForce;
	[Header("Force Y")]
	[Tooltip("Minimum force on Y axis")]
	public float minimumYForce;
	[Tooltip("Maximum force on Y axis")]
	public float maximumYForce;
	[Header("Force Z")]
	[Tooltip("Minimum force on Z axis")]
	public float minimumZForce;
	[Tooltip("Maximum force on Z axis")]
	public float maximumZForce;
	[Header("Rotation Force")]
	[Tooltip("Minimum initial rotation value")]
	public float minimumRotation;
	[Tooltip("Maximum initial rotation value")]
	public float maximumRotation;
	[Header("Despawn Time")]
	[Tooltip("How long after spawning that the casing is destroyed")]
	public float despawnTime;

	[Header("Audio")]
	public AudioClip[] casingSounds;
	public AudioSource audioSource;

	[Header("Spin Settings")]

	[Tooltip("How fast the casing spins over time")]
	public float speed = 2500.0f;

	
	private void Awake () 
	{
		//Nasumična rotacija metka
		GetComponent<Rigidbody>().AddRelativeTorque (
			Random.Range(minimumRotation, maximumRotation), 
			Random.Range(minimumRotation, maximumRotation), 
			Random.Range(minimumRotation, maximumRotation)  
			* Time.deltaTime);

		//Nasumičan smjer metka
		GetComponent<Rigidbody>().AddRelativeForce (
			Random.Range (minimumXForce, maximumXForce),  
			Random.Range (minimumYForce, maximumYForce),  
			Random.Range (minimumZForce, maximumZForce)); 	     
	}

	private void Start () 
	{
		//Uništavanje metka
		StartCoroutine (RemoveCasing ());
		//Postavljanje nasumične pozicije
		transform.rotation = Random.rotation;
		//zvuk pucnja
		StartCoroutine (PlaySound ());
	}

	private void FixedUpdate () 
	{
		//rotiranje metka prema brzini
		transform.Rotate (Vector3.right, speed * Time.deltaTime);
		transform.Rotate (Vector3.down, speed * Time.deltaTime);
	}
	//funkcija za zvuk metka
	private IEnumerator PlaySound () 
	{
		
		yield return new WaitForSeconds (Random.Range(0.25f, 0.85f));
		
		audioSource.clip = casingSounds
			[Random.Range(0, casingSounds.Length)];
		
		audioSource.Play();
	}
	//funkcija za uništavanje metka
	private IEnumerator RemoveCasing () 
	{
		
		yield return new WaitForSeconds (despawnTime);
	
		Destroy (gameObject);
	}
}
