using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] planetPrefabs;
    public GameObject[] meteorPrefabs;
    public GameObject[] powerPrefabs;
    public GameObject[] starPrefabs;

    private bool bIsSpawning = false;

    private PlayerController player;
    private GameManager gameHQ;
    //[] directive makes it available in the Editor Inspector
    [SerializeField] private int minSpawnAmount = 5;//used for spawn triggering
    [SerializeField] private int maxSpawnAmount = 20;
    //No longer time based spawning but still here for now
    private float maxSpawnTime;
    private float spawnTime;

    private float minSpawnZ = -100f;
    private float maxSpawnZ = -400f;
    private float spawnZMoveMultiplier = 2f;
    private float maxSpawnY;
    private float maxSpawnX;
    //so we can move the z-index after first birth
    //private bool bPlanetsSpawned = false, bMeteorsSpawned = false, bStarsSpawned = false, bPowerSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        

        //RestartSpawn();
    }

    void Awake(){
        //reference to player
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        maxSpawnY = player.GetBounds().y;
        //Debug.Log("maxSpawnY: "+maxSpawnY);
        maxSpawnX = player.GetBounds().x;
        //Debug.Log("maxSpawnX: "+maxSpawnX);
        //reference to game manager
        gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //If there are too few meteors then spawn another wave of everything
        //++ needs improving - now better with more speed variation
        if(FindObjectsOfType<MeteorBehaviour>().Length < minSpawnAmount * gameHQ.GetDifficulty() && bIsSpawning){
            SpawnAll();
        }
    }
    
    public float GetMaxSpawnZ(){
        return maxSpawnZ;
    }

    private void ResetSpawnZ(){
        maxSpawnZ /= spawnZMoveMultiplier;
        minSpawnZ -= maxSpawnZ;
    }

    public void RestartSpawn(){
        SpawnAll();
        //... then move the z spawn backwards as in Start()
        minSpawnZ += maxSpawnZ;
        maxSpawnZ *= spawnZMoveMultiplier;
        bIsSpawning = true;
    }

    public void StopSpawning(){
        bIsSpawning = false;
        ResetSpawnZ();
    }

    void SpawnAll(){
        //make it based on a difficulty, so as it gets more difficult you get more meteors and less power ups
        SpawnMeteors(minSpawnAmount*gameHQ.GetDifficulty(),maxSpawnAmount*gameHQ.GetDifficulty());
        //we want this to be controllable so less as it gets harder
        SpawnPowerUps(minSpawnAmount/gameHQ.GetDifficulty(),maxSpawnAmount/gameHQ.GetDifficulty());
        //both of these are mainly for the enviromental aesthetic
        //++make them damage massively if hit 
        SpawnPlanets(1,3);
        SpawnStars(minSpawnAmount/3,maxSpawnAmount/3);
    }

    void SpawnPlanets(int min, int max){
        //we don't want many planets, they're rather rare
        int totalSpawn = Random.Range(min,max);
        //Planets can be outside of bounds for environmental decoration        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,planetPrefabs.Length);
            float randomX = Random.Range(-maxSpawnX*10,maxSpawnX*10);
            float randomY = Random.Range(-maxSpawnY*10,maxSpawnY*10);
            float randomZ = Random.Range(minSpawnZ,maxSpawnZ);
            Vector3 birthPos = new Vector3(randomX,randomY,randomZ);
            Instantiate(planetPrefabs[randomSelection],birthPos,planetPrefabs[randomSelection].transform.rotation);
        }
    }

    void SpawnMeteors(int min, int max){
        int totalSpawn = Random.Range(min,max);
        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,meteorPrefabs.Length);
            float randomX = Random.Range(-maxSpawnX,maxSpawnX);
            float randomY = Random.Range(-maxSpawnY,maxSpawnY);
            float randomZ = Random.Range(minSpawnZ,maxSpawnZ);
            Vector3 birthPos = new Vector3(randomX,randomY,randomZ);
            Instantiate(meteorPrefabs[randomSelection],birthPos,meteorPrefabs[randomSelection].transform.rotation);
        }
    }
    void SpawnStars(int min, int max){
        //less stars than other things
        int totalSpawn = Random.Range(min,max);
        //Stars can be outside of bounds for environmental decoration
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,starPrefabs.Length);
            float randomX = Random.Range(-maxSpawnX*10,maxSpawnX*10);
            float randomY = Random.Range(-maxSpawnY*10,maxSpawnY*10);
            float randomZ = Random.Range(minSpawnZ,maxSpawnZ);
            Vector3 birthPos = new Vector3(randomX,randomY,randomZ);
            Instantiate(starPrefabs[randomSelection],birthPos,starPrefabs[randomSelection].transform.rotation);
        }
    }
    void SpawnPowerUps(int min, int max){
        int totalSpawn = Random.Range(min,max);
        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,powerPrefabs.Length);
            float randomX = Random.Range(-maxSpawnX,maxSpawnX);
            float randomY = Random.Range(-maxSpawnY,maxSpawnY);
            float randomZ = Random.Range(minSpawnZ,maxSpawnZ);
            Vector3 birthPos = new Vector3(randomX,randomY,randomZ);
            Instantiate(powerPrefabs[randomSelection],birthPos,powerPrefabs[randomSelection].transform.rotation);
        }
    }
}
