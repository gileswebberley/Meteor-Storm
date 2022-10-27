using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using IModels;

public class MeteorBehaviour : MoveForward, ISpawnedEnemy//MonoBehaviour, ISpawnedEnemy
{
    public float power = 2f;
    private float startPower;
    //when we are destroyed play this particle system in our position
    public ParticleSystem explodePS;
    //when we're hit check how powerful the laser is that hit us by checking player
    //private PlayerController player;
    //so we can add our score via the GameManager
    private GameManager gameHQ;

    //implement ISpawnedEnemy spawn
    protected ISpawnable _spawn;
    public ISpawnable spawn{
        get{return _spawn;}
        protected set {_spawn = value;}
    } 

    // Start is called before the first frame update
    protected override void Start()
    {
        //set up player and thisRB along with randomise the speed variable
        base.Start();
        
        //for scoring
        startPower = power;
        gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //for spawn manager count
        AddToSpawn();

        //add a bit of random rotation on birth using thisRB set in base start()
        thisRB.AddTorque(Random.Range(-5,5),Random.Range(-5,5),Random.Range(-5,5),ForceMode.Force);
    }

    protected override void Update()
    {
        //we want to change the death behaviour so don't run base.Update();
        if(!RbAddForwardForce()){//means we're out of GameBounds.z
            RemoveFromSpawn();
            //Destroy(gameObject);
        }
    }

    //this is not right is it, I want to make sure that spawn manager keeps track of our death 
    public void RemoveFromSpawn(){
        --spawn.currentSpawnedEnemies;
        //we've removed ourselves so we must be dead, let's ensure that
        Destroy(gameObject);
    }

    public void AddToSpawn(){
        //set up our reference to an ISpawnable (which has a currentSpawnedEnemies property)
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        //add ourselves to the spawn manager's internal count
        ++spawn.currentSpawnedEnemies;
    }

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("LaserShot")){
            //hit by a laser, check how powerful it is
            power -= player.GetLaserPower();
            //remove laser shot that hit
            Destroy(other.gameObject);
            if(power <= 0){
                //can't get the particle system to work :( ++ There we go, simply use Instantiate rather than try to play it
                Instantiate(explodePS,transform.position, transform.rotation);
                //add my original power to the player score
                gameHQ.UpdateScore(((int)startPower));//warning casting to int
                RemoveFromSpawn();
                //destroyed
                //Destroy(gameObject);
            }
        }
    }
}
