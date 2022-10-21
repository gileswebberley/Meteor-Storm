using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f;
    private float speedRandomiserRange = 7f;
    private float speedWithinEnvironment = 0f;
    //to discover the speed of the player 
    private PlayerController player;
    //to discover the z bounds that is being spawned at so they die if they go further
    private SpawnManager spawn;
    private Rigidbody thisRB;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        thisRB = GetComponent<Rigidbody>();
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        //if it's not a laser then randomise my speed a bit
        if(!gameObject.CompareTag("LaserShot")){
            speed = Random.Range(speed/speedRandomiserRange,speed*speedRandomiserRange);
        }
    }

    // Update is called once per frame
    void Update()
    {    
        speedWithinEnvironment = speed*player.GetSpeed();
        //This is how to get a continuous force that's just like the Translate behaviour
        thisRB.AddForce(Vector3.forward*speedWithinEnvironment,ForceMode.Force);    
        //transform.Translate(Vector3.forward*(speed*player.GetSpeed())*Time.deltaTime);

        //check that we are still in view (within z-bounds)
        if(transform.position.z > player.transform.position.z + 10 || transform.position.z < spawn.spawnProperties.MaxSpawnZ){
            Destroy(gameObject);
        }
    }
}
