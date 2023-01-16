using System.ComponentModel;
using UnityEngine;
using IModels;

/*
This is the abstract base class that implements the ISpawnable interface and brings
the Unity MonoBehaviour into the inheritance heirarchy.
*/

public abstract class SpawnerBase : MonoBehaviour, ISpawnable {
    //Encapsulate so that members can be read from anywhere but only written in sub-class
    //Vital boolean set to a default value of false
    private bool _bHasStarted = false;
    public bool bHasStarted {
        get {return _bHasStarted;}
        protected set {_bHasStarted = value;}
    }
    private bool _bIsSpawning = false;
    public bool bIsSpawning {
        get {return _bIsSpawning;}
        protected set { _bIsSpawning = value;}
    }
    //[SerializeField] directive makes it available in the Editor Inspector
    //Doesn't seem to pass down the inheritance chain...:/
    //Minimum amount to spawn in a wave
    [SerializeField] public int minSpawnAmount {get; protected set;} = 5;   
    //Maximum amount to spawn in a wave
    [SerializeField] public int maxSpawnAmount {get; protected set;} = 20;
    //this allows public visibility of maxSpawnX but is protected access
    //for setting (within class or derived classes)
    public float minSpawnX { get; protected set; }
    public float maxSpawnX { get; protected set; }
    public float minSpawnY { get; protected set; }
    public float maxSpawnY { get; protected set; }
    //Encapsulation with value checking through Property Methods
    protected float _minSpawnZ = -100f;
    public float minSpawnZ {
        get {return _minSpawnZ;} 
        protected set { _minSpawnZ = value;}
    }
    protected float _maxSpawnZ = -400f;
    public float maxSpawnZ {
        get {return _maxSpawnZ;} 
        protected set { _maxSpawnZ = value;}
    }

    private int _currentSpawnedEnemies = 0;
    public int currentSpawnedEnemies{
        //how do I make this thread safe with a lock?
        get{return _currentSpawnedEnemies;}
        set {
            Debug.Log("currentSpawnedEnemies: "+_currentSpawnedEnemies);
            //does this allow for pre and or post incrementing? seems so :)if(value > 0) 
            _currentSpawnedEnemies = value;
        }
    }
    //abstract classes that must be overriden and implemented in the child class
    public abstract void StartSpawn();
    public abstract void RestartSpawn();
    public abstract void StopSpawning();
    public abstract void SpawnAll();
    public abstract void SpawnEnemies();
    public abstract void SpawnBonuses();

    //Handy utility function for children, marked virtual so that it CAN be overridden if required
    protected virtual Vector3 GetRandomStartPosition(float BoundsSizeMultiplier = 1f)
    {
        float randomX = Random.Range(minSpawnX*BoundsSizeMultiplier,maxSpawnX*BoundsSizeMultiplier);
        float randomY = Random.Range(minSpawnY*BoundsSizeMultiplier,maxSpawnY*BoundsSizeMultiplier);
        float randomZ = Random.Range(minSpawnZ*BoundsSizeMultiplier,maxSpawnZ*BoundsSizeMultiplier);
        Vector3 birthPos = new Vector3(randomX,randomY,randomZ);
        return birthPos;
    }
}