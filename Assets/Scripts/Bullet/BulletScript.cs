using UnityEngine;
using System.Collections;


public class BulletScript : MonoBehaviour {

	[Range(5, 100)]
	[Tooltip("After how long time should the bullet prefab be destroyed?")]
	public float destroyAfter;
	[Tooltip("If enabled the bullet destroys on impact")]
	public bool destroyOnImpact = false;
	[Tooltip("Minimum time after impact that the bullet is destroyed")]
	public float minDestroyTime;
	[Tooltip("Maximum time after impact that the bullet is destroyed")]
	public float maxDestroyTime;

	[Header("Impact Effect Prefabs")]
	public Transform [] metalImpactPrefabs;
	public int enemyHealth = 100;

	private void Start () 
	{
		//Startanje timera uništenja metka
		StartCoroutine (DestroyAfter ());
	}

	//funkcija za provjeru dal se metak sudara s objektima
	private void OnCollisionEnter (Collision collision) 
	{
		//provjerava se ako je destroy on impact lažan, ako jest, počinje uništenje metaka
		if (!destroyOnImpact) 
		{
			StartCoroutine (DestroyTimer ());
		}
		//ako je istinit, uništava se metak
		else 
		{
			Destroy (gameObject);
		}

		

	
	

		if (collision.gameObject.CompareTag("Enemy")) {

			collision.gameObject.GetComponent<SC_NPCEnemy>().ApplyDamage(25);
			
			
		}
	}

	private IEnumerator DestroyTimer () 
	{
		//Čekanje nasumičnog vremena za uništenje objekta
		yield return new WaitForSeconds
			(Random.Range(minDestroyTime, maxDestroyTime));
		//Uništenje metka
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfter () 
	{
		//Ćekanje određenog vremena
		yield return new WaitForSeconds (destroyAfter);
		//Uništenje metka
		Destroy (gameObject);
	}
}
