using UnityEngine;
using System.Collections;


public class Bullet : MonoBehaviour {

	[Range(5, 100)]
	public float destroyAfter;
	public bool destroyOnImpact = false;
	public float minDestroyTime;
	public float maxDestroyTime;



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
	//provjerana se kolizija s neprijateljskim objektom
	//ako se sudara, oduzima neprijatelju 25 zdravlja

		if (collision.gameObject.CompareTag("Enemy")) {

			collision.gameObject.GetComponent<Enemy>().ApplyDamage(25);						
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
