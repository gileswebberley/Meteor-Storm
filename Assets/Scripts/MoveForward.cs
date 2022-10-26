using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//for GameBounds which must be set in GameManger
using OodModels;

[RequireComponent(typeof(Rigidbody))]
public class MoveForward : MonoBehaviour
{
    public float speed = 5f;
    private float speedRandomiserRange = 7f;
    private float speedWithinEnvironment = 0f;
    //to discover the speed of the player 
    private PlayerController player;
    //This is the declared required component
    private Rigidbody thisRB;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        thisRB = GetComponent<Rigidbody>();
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

        //check that we are still in view (within z-bounds)
        if(transform.position.z > GameBounds.minZ || transform.position.z < GameBounds.maxZ){
            Destroy(gameObject);
        }
    }
}
