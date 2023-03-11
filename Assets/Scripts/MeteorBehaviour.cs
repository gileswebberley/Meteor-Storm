using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
using GilesSpawnSystem;
using GilesScoreSystem;
using GilesManagers;
//this seems to have made Start() etc throw up problems
//using GilesMovement;

public class MeteorBehaviour : RandomSpeedMoveForwardsRb, ISpawnedEnemy
{
    public float power = 2f;
    private float startPower;
    //when we are destroyed play this particle system in our position
    public ParticleSystem explodePS;
    public AudioClip explodeSound;
    //when we're hit check how powerful the laser is that hit us by checking player
    //private PlayerController player;
    //so we can add our score via the GameManager
    //private GameManager gameHQ; - now using GilesManagers.GameProperties

    //implement ISpawnedEnemy spawn
    protected ISpawnable _spawn;
    public ISpawnable spawn
    {
        get { return _spawn; }
        protected set { _spawn = value; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        //set up player and thisRB along with randomise the speed variable
        base.Start();

        //for scoring
        startPower = power;
        //gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //for spawn manager count
        AddToSpawn();

        //add a bit of random rotation on birth using thisRB set in base start()
        thisRB.AddTorque(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5), ForceMode.Force);
    }

    protected override void FixedUpdate()
    {//this checks that the GameObject will add/remove itself from the counter
        //we want to change the death behaviour so don't run base.FixedUpdate();
        if (!RbAddForce(Vector3.forward))
        {//means we're out of GameBounds.z//this checks that the GameObject will add/remove itself from the counter
            RemoveFromSpawn();
        }
    }

    void PlaySound()
    {
        //check if we have a sound to play on death and if we have access to an AudioManager
        string qualifiedName = typeof(AudioManager).AssemblyQualifiedName;
        if (explodeSound != null && System.Type.GetType(qualifiedName) != null) AudioManager.PlaySoundFromPosition(transform.position, explodeSound);
    }

    //this is the ISpawnedEnemy stuff, I want to make sure that spawn manager keeps track of our death 
    public void RemoveFromSpawn()
    {
        //PlaySound();
        //we've removed ourselves so we must be dead, let's ensure that
        Destroy(gameObject);
        //now removing from spawn count is done in OnDestroy()
    }

    public void AddToSpawn()
    {
        //set up our reference to an ISpawnable (which has a currentSpawnedEnemies property)
        spawn = GameObject.Find(TagManager.SPAWN).GetComponent<SpawnManager>() as ISpawnable;
        if (spawn == null)
        {
            Debug.LogError("MeteorBehaviour.AddToSpawn cannot find an ISpawnable reference");
            return;
        }
        //add ourselves to the spawn manager's internal count
        ++spawn.currentSpawnedEnemies;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagManager.BULLET))
        {
            //hit by a laser, check how powerful it is
            power -= other.gameObject.GetComponent<LaserBehaviour>().laserPower;//GetLaserPower();
            //remove laser shot that hit
            Destroy(other.gameObject);
            if (power <= 0)
            {
                //can't get the particle system to work :( ++ There we go, simply use Instantiate rather than try to play it
                Instantiate(explodePS, transform.position, transform.rotation);
                PlaySound();
                //add my original power to the player score
                ScoringSystem.Instance.Scorer.AddToScore((int)startPower);//warning casting to int
                RemoveFromSpawn();
            }
        }
    }

    void OnDestroy()
    {
        //perhaps removing from the spawn count should happen here although it could end up causing an awful loop?
        //if we haven't been added we won't have a reference to spawn, as it should be I think
        if (spawn == null)
        {
            Debug.LogError("RemoveFromSpawn called without ISpawnable reference");
            return;
        }
        //move this to OnDestroy 
        --spawn.currentSpawnedEnemies;
    }
}
