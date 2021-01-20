using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Health player;
    public Texture crosshairTexture;
    public float spawnInterval = 2; //Spawn new enemy each n seconds
    public int enemiesPerWave = 5; //How many enemies per wave
    public Transform[] spawnPoints;
    public Transform playerPosition;
    //public Transform spawnPosition;

    float nextSpawnTime = 0;
    int waveNumber = 1;
    bool waitingForWave = true;
    float newWaveTimer = 0;
    int enemiesToEliminate;
    //How many enemies we already eliminated in the current wave
    int enemiesEliminated = 0;
    int totalEnemiesSpawned = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Wait 10 seconds for new wave to start
        newWaveTimer = 5;
        waitingForWave = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingForWave)
        {
            if (newWaveTimer >= 0)
            {
                newWaveTimer -= Time.deltaTime;
            }
            else
            {
                //Initialize new wave
                enemiesToEliminate = waveNumber * enemiesPerWave;
                enemiesEliminated = 0;
                totalEnemiesSpawned = 0;
                waitingForWave = false;
            }
        }
        else
        {
            if (Time.time > nextSpawnTime)
            {
                nextSpawnTime = Time.time + spawnInterval;

                //if (Vector3.Distance(playerPosition.transform.position, transform.transform.position ) < 30f) {

                    //Spawn enemy 
                    if (totalEnemiesSpawned < enemiesToEliminate)
                    {
                        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];

                        GameObject enemy = Instantiate(enemyPrefab, randomPoint.position, Quaternion.identity);
                        SC_NPCEnemy npc = enemy.GetComponent<SC_NPCEnemy>();
                        npc.playerTransform = player.transform;
                        npc.es = this;
                        totalEnemiesSpawned++;
                    }
                //}
            }
        }

        if (player.currentHealth <= 0)
        {
           
                LevelMenager man = GameObject.Find("LevelMenager").GetComponent<LevelMenager>();
                man.LoadLevel("Lose");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;


            
        }
    }

    void OnGUI()
    {
       
      

        GUI.Box(new Rect(Screen.width / 2 - 50, 10, 100, 25), (enemiesToEliminate).ToString());

        if (waitingForWave)
        {
            GUI.Box(new Rect(Screen.width / 2 - 125, Screen.height / 4 - 12, 250, 25), "Waiting for Wave " + waveNumber.ToString() + ". " + ((int)newWaveTimer).ToString() + " seconds left...");
        }
    }

    public void EnemyEliminated(SC_NPCEnemy enemy)
    {
        enemiesEliminated++;

        if (enemiesToEliminate - enemiesEliminated <= 0)
        {
            //Start next wave
            newWaveTimer = 5;
            waitingForWave = true;
            waveNumber++;
        }
    }
}