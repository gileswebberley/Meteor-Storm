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
    //public SpawnProperties spawnProperties { get => spawnProperties; protected set => spawnProperties = value;}

    //spawnProperties = new SpawnProperties;

    //[] directive makes it available in the Editor Inspector
    //[SerializeField] private int minSpawnAmount = 5;//used for spawn triggering
    //[SerializeField] private int maxSpawnAmount = 20;
    //No longer time based spawning but still here for now - maybe this should be a seperate TimeSpawnable interface?
    private float maxSpawnTime;
    private float spawnTime;

    // private float _minSpawnZ = -100f;
    // //Encapsulation with value checking through Property Methods
    // public float _minSpawnZ {
    //     get {return _minSpawnZ;} 
    //     protected set {
    //                 if(value <= 0){ 
    //                     //make sure it's a negative value
    //                     _minSpawnZ = value;
    //                 }else{
    //                     //assume user forgot the minus sign
    //                     _minSpawnZ = value * -1;
    //                 }
    //             }
    // }
    // private float maxSpawnZ = -400f;
    // public float maxSpawnZ {
    //     get {return maxSpawnZ;} 
    //     protected set {
    //                 if(value <= 0){ 
    //                     //make sure it's a negative value
    //                     maxSpawnZ = value;
    //                 }else{
    //                     //assume user forgot the minus sign
    //                     maxSpawnZ = value * -1;
    //                 }
    //             }
    // }
    //This is also vital for Spawnable behaviour but 
    //Interfaces are for "implementing peripheral abilities" - GeeksForGeeks
    private float spawnZMoveMultiplier = 2f;

    //An example of using C# Properties to encapsulate a field
    // private float maxSpawnX;
    // //this allows public visibility as MaxSpawnX but is protected access for setting (within class or derived classes)
    // public float MaxSpawnX { get => maxSpawnX; protected set => maxSpawnX = value; }
    // private float maxSpawnY;
    // public float MaxSpawnY { get => maxSpawnY; protected set => maxSpawnY = value; }

    //so we can move the z-index after first birth
    //private bool bPlanetsSpawned = false, bMeteorsSpawned = false, bStarsSpawned = false, bPowerSpawned = false;
    // Start is called before the first frame update
    void Start()
    {
        

        //RestartSpawn();
    }

    void Awake(){
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

    //Public interface of Spawnable
    public override void RestartSpawn(){
        SpawnAll();
        Debug.Log("SpawnAll() called from RestartSpawn() : bIsSpawning = "+bIsSpawning);
        //... then move the z spawn backwards as in Start()
        minSpawnZ += maxSpawnZ;
        maxSpawnZ *= spawnZMoveMultiplier;
        bIsSpawning = true;
    }
    //Public interface of Spawnable
    public override void StopSpawning(){
        Debug.Log("StopSpawning() called");
        bIsSpawning = false;
        ResetSpawnZ();
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
