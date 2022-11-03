using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//for GameBounds which must be set in GameManger
using OodModels;

//using this directive makes Unity Editor add the component to the game object
//it's attached to if missing
[RequireComponent(typeof(Rigidbody))]
public class MoveForwardRb : MonoBehaviour
{
    public float speed = 5f;
    
    protected float speedWithinEnvironment = 0f;
    //to discover the speed of the player 
    protected PlayerController player;
    //This is the declared required component
    protected Rigidbody thisRB;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if(player == null) Debug.LogError("MoveForwardRb cannot find Player component PlayerController");
        thisRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {  
        //check for bounds and kill if false  
        if(!RbAddForwardForce()){
            Destroy(gameObject);
        }

    }

    protected virtual bool RbAddForwardForce()
    {  
        speedWithinEnvironment = speed*player.GetSpeed();
        //This is how to get a continuous force that's just like the Translate behaviour
        thisRB.AddForce(Vector3.forward*speedWithinEnvironment,ForceMode.Force);
        //check that we are still in view (within z-bounds)
        if(transform.position.z > GameBounds.minZ || transform.position.z < GameBounds.maxZ){
            return false;
        }
        //we are within bounds and have moved forwards
        return true;
    }
}