using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour, IEntity
{
    //postavljanje varijabli
    public float attackDistance = 3f;
    public float movementSpeed = 4f;
    public float npcHP = 100;
    public Health player;
    public float npcDamage = 5;
    public float attackRate = 0.5f;
    public Transform firePoint;
    public GameObject npcDeadPrefab;
    public float range = 20f;
    public AudioClip dead;
    public float volume = 100; 
    public Transform playerTransform;
    [HideInInspector]
    NavMeshAgent agent;
    float nextAttackTime = 0;
    Rigidbody r;

 //Postavljanje Agenta
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackDistance;
        agent.speed = movementSpeed;
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
        r.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance - attackDistance < 0.01f)
        {
            if (Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;

               //Provjerava ako je metak pogodio neprijatelja, ako je tvrdnja istinita, neprijatelj gubi zdravlje
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, attackDistance))
                {
                    if (hit.transform.CompareTag("Bullet"))
                    {
                        Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * attackDistance, Color.cyan);

                        IEntity player = hit.transform.GetComponent<IEntity>();
                        player.ApplyDamage(npcDamage);
                       
                    }
                }
            }
        }
        //provjerava ako je zdravlje igrača manje ili jednako 0
        //učitava se lose scena
        if (player.currentHealth <= 0)
        {

            LevelMenager man = GameObject.Find("LevelMenager").GetComponent<LevelMenager>();
            man.LoadLevel("Lose");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        //Provjeravanje ako je igrač u dosegu igrača, ako jest počne se kretati prema njemu
        if (Vector3.Distance(playerTransform.transform.position, transform.transform.position) <= range) {

            agent.destination = playerTransform.position;
        }
    
        //Pogled na igrača
        transform.LookAt(new Vector3(playerTransform.transform.position.x, transform.position.y, playerTransform.position.z));
        
        r.velocity *= 0.99f;
    }


    //funkcija za oduzimanje zdravlja neprijatelja
    public void ApplyDamage(float points)
    {
        npcHP -= points;
        if (npcHP <= 0)
        {
            
            //Uništavanje neprijatelja
            GameObject npcDead = Instantiate(npcDeadPrefab, transform.position, transform.rotation);
            

            //Padanje mrtvog prefaba neprijatelja prema dolje
            npcDead.GetComponent<Rigidbody>().velocity = (-(playerTransform.position - transform.position).normalized * 8) + new Vector3(0, 5, 0);
            Destroy(npcDead, 10);
            //Zvuk smrti neprijatelja
            AudioSource.PlayClipAtPoint(dead, transform.position, 10f);
          //Uništavanje neprijatelja
            Destroy(gameObject);
            
        }
    }


    //Funkcija za provjeravanje kolizije s igračem, ako je kolizija istinita, oduzima igraču zdravlje
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Health>().ApplyDamage(10);
            

        }
    }


}