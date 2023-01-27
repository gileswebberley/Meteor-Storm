using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//for GameBounds which are set in GameManager
using GilesManagers;

//using this directive makes Unity Editor add the component to the game object
//it's attached to if missing
[RequireComponent(typeof(Rigidbody))]
public class MoveForwardRb : MonoBehaviour
{
    public float speed = 5f;
    //just for fine tuning the movement
    [SerializeField] float speedModifier = 5f;
    
    protected float speedWithinEnvironment = 0f;
    //to discover the speed of the player - now in GameProperties
    //protected PlayerController player;
    //This is the declared required component
    protected Rigidbody thisRB;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //this is bad coding practise I think, is it called coupling?
        // player = GameObject.Find(TagManager.PLAYER).GetComponent<PlayerController>();
        // if(player == null) Debug.LogError("MoveForwardRb cannot find Player component PlayerController");
        thisRB = GetComponent<Rigidbody>();
    }

    //in FixedUpdate so that pausing with Time.timeScale doesn't cause errors and because it is physics related
    protected virtual void FixedUpdate()
    {
        //check for bounds and kill if false  
        if(!RbAddForce(Vector3.forward)){
            Destroy(gameObject);
        }

    }

    protected virtual bool RbAddForce(Vector3 directionVector)
    {  
        float tmp = speedWithinEnvironment;
        speedWithinEnvironment = speed + GameProperties.Instance.GetGameProperty<int>(TagManager.PLAYER_SPEED_PROPERTY);
        //if we've slowed since last time then add a bit of reverse velocity change - because of this change it no longer works with a negative speed value (which I was using for the laser shot)
        if(tmp > speedWithinEnvironment) thisRB.AddForce(directionVector*((tmp-speedWithinEnvironment)*-speedModifier),ForceMode.VelocityChange);
        //if we've sped up then add a velocity change
        if(speedWithinEnvironment > tmp) thisRB.AddForce(directionVector*(speedWithinEnvironment-tmp)*speedModifier,ForceMode.VelocityChange);
        
        //check that we are still in view (within z-bounds)
        if(transform.position.z > GameBounds.minZ || transform.position.z < GameBounds.maxZ){
            return false;
        }

        //we are within bounds after moving forwards
        return true;
    }
}