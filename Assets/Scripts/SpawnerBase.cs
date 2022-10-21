using System.ComponentModel;
using UnityEngine;

public abstract class SpawnerBase : MonoBehaviour, ISpawnable {
    //Encapsulate so that members can be read from anywhere but only written in sub-class
    //Vital boolean set to a default value of false using System.ComponentModel
    protected bool _bIsSpawning = false;
    public bool bIsSpawning {
        get {return _bIsSpawning;}
        protected set { _bIsSpawning = value;}
    }
    //Minimum amount to spawn in a wave
    //[DefaultValue(5)]
    [SerializeField] public int minSpawnAmount {get; protected set;} = 5;   
    //Maximum amount to spawn in a wave
    //[DefaultValue(20)]
    [SerializeField] public int maxSpawnAmount {get; protected set;} = 20;
    //this allows public visibility as MaxSpawnX but is protected access for setting (within class or derived classes)
    public float maxSpawnX { get; protected set; }
    public float maxSpawnY { get; protected set; }
    //Encapsulation with value checking through Property Methods
    protected float _minSpawnZ = -100f;
    //[DefaultValue(-100f)]
    public float minSpawnZ {
        get {return _minSpawnZ;} 
        protected set {
                    if(value <= 0){ 
                        //make sure it's a negative value
                        _minSpawnZ = value;
                    }else{
                        //assume user forgot the minus sign
                        _minSpawnZ = value * -1;
                    }
                }
    }
    protected float _maxSpawnZ = -400f;
    //[DefaultValue(-400f)]
    public float maxSpawnZ {
        get {return _maxSpawnZ;} 
        protected set {
                    if(value <= 0){ 
                        //make sure it's a negative value
                        _maxSpawnZ = value;
                    }else{
                        //assume user forgot the minus sign
                        _maxSpawnZ = value * -1;
                    }
                }
    }

    public abstract void RestartSpawn();
    public abstract void StopSpawning();
    public abstract void SpawnAll();
}