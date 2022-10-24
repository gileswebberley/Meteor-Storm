using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    perhaps x, y, and z bounds should be part of the GameManager with which
    other classes can communicate? Just thinking cos in here there's a reference
    to the Player game object just to get the x and y bounds, which are more for a
    GameWorld type object? Maybe a struct (or interface of properties) within GameManager?
*/
public class SpawnManager : SpawnerBase
{
    //3DSpaceSpawn specific library of objects to be spawning
    public GameObject[] planetPrefabs;
    public GameObject[] meteorPrefabs;
    public GameObject[] powerPrefabs;
    public GameObject[] starPrefabs;

    //everything wants links to Player and GameManager so perhaps this should be some kind of utility class of it's own 
    //should player be available through the GameManager rather than directly from all these classes?
    private PlayerController player;
    private GameManager gameHQ;
    //No longer time based spawning but still here for now - maybe this should be a seperate TimeSpawnable interface?
    private float maxSpawnTime;
    private float spawnTime;
    //This is also vital for Spawnable behaviour but 
    //Interfaces are for "implementing peripheral abilities" - GeeksForGeeks
    

    //so we can move the z-index after first birth
    private float spawnZMoveMultiplier = 2f;
    

    void Start()
    {
        

        //RestartSpawn();
    }

    void Awake(){
        minSpawnX = -maxSpawnX;
        minSpawnY = -maxSpawnY;
        Debug.Log("SpawnManager Awake()");
        //reference to player
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        //This doesn't make sense - player should be getting these from here, perhaps via GameManager?
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
        if(!bIsSpawning) return;
        //If there are too few meteors then spawn another wave of everything
        //++ needs improving - now better with more speed variation in MoveForward
        if(FindObjectsOfType<MeteorBehaviour>().Length < minSpawnAmount * gameHQ.GetDifficulty()){
            SpawnAll();
            Debug.Log("SpawnAll() called from Update()");
        }
    }

    //Abstraction of the resetting of fields to a start state
    private void ResetSpawnZ(){
        Debug.Log("ResetSpawn() called");
        maxSpawnZ /= spawnZMoveMultiplier;
        minSpawnZ -= maxSpawnZ;
    }

    public override void StartSpawn(){
        SpawnAll();
        Debug.Log("SpawnAll() called from StartSpawn() ");
        //... then move the z spawn backwards as in Start()
        minSpawnZ += maxSpawnZ;
        maxSpawnZ *= spawnZMoveMultiplier;
        bIsSpawning = true;
        bHasStarted = true;
    }

    //Public interface of Spawnable
    public override void RestartSpawn(){
        if(bHasStarted){
            ResetSpawnZ();
        }
        StartSpawn();
    }
    //Public interface of Spawnable
    public override void StopSpawning(){
        Debug.Log("StopSpawning() called");
        bIsSpawning = false;
        //ResetSpawnZ();
    }
    //Abstraction that every Spawnable must implement - this should be split into SpawnEnemies and SpawnBonuses
    public override void SpawnAll(){
        int diff = gameHQ.GetDifficulty();
        //make it based on a difficulty, so as it gets more difficult you get more meteors and less power ups
        SpawnMeteors(minSpawnAmount*diff,maxSpawnAmount*diff);
        //we want this to be controllable so less as it gets harder
        SpawnPowerUps(minSpawnAmount/diff,maxSpawnAmount/diff);
        //both of these are mainly for the enviromental aesthetic
        //++make them damage massively if hit 
        SpawnPlanets(1,3);
        SpawnStars(minSpawnAmount/3,maxSpawnAmount/3);
    }
    //These could be derived class (3DSpaceSpawner) behaviour that is put in to honour our Spawnability
    void SpawnPlanets(int min, int max){
        //we don't want many planets, they're rather rare
        int totalSpawn = Random.Range(min,max);
        //Planets can be outside of bounds for environmental decoration        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,planetPrefabs.Length);
            //use the utility function inherited from SpawnerBase
            Vector3 birthPos = GetRandomStartPosition(10f);
            birthPos.z /= 10f;
            Instantiate(planetPrefabs[randomSelection],birthPos,planetPrefabs[randomSelection].transform.rotation);
        }
    }

    void SpawnMeteors(int min, int max){
        int totalSpawn = Random.Range(min,max);
        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,meteorPrefabs.Length);
            Vector3 birthPos = GetRandomStartPosition();
            Instantiate(meteorPrefabs[randomSelection],birthPos,meteorPrefabs[randomSelection].transform.rotation);
        }
    }
    void SpawnStars(int min, int max){
        //less stars than other things
        int totalSpawn = Random.Range(min,max);
        //Stars can be outside of bounds for environmental decoration
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,starPrefabs.Length);
            Vector3 birthPos = GetRandomStartPosition(10f);
            birthPos.z /= 10f;
            Instantiate(starPrefabs[randomSelection],birthPos,starPrefabs[randomSelection].transform.rotation);
        }
    }
    void SpawnPowerUps(int min, int max){
        int totalSpawn = Random.Range(min,max);
        
        for(int i = 0; i < totalSpawn; i++){
            int randomSelection = Random.Range(0,powerPrefabs.Length);
            Vector3 birthPos = GetRandomStartPosition();
            Instantiate(powerPrefabs[randomSelection],birthPos,powerPrefabs[randomSelection].transform.rotation);
        }
    }

}
