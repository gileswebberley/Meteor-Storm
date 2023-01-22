using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//for the GameBounds singleton
using OodModels;
using IModels;
using GilesManagers;

[RequireComponent(typeof(StrengthManager))]
[RequireComponent(typeof(PowerManager))]
[RequireComponent(typeof(LaserWeapon))]

public class PlayerController : MonoBehaviour
{
    //very important boolean - possible for IPlayable me thinks
    private bool bIsPlaying = false;

    //Movement related control system
    private Rigidbody playerRB;
    //all the speed based properties -------
    //affects control twitchyness, other objects use it to make it feel like speed has changed
    private float speed = 10f;
    public float Speed{
        get{return speed;}
    }
    private float minSpeed = 5f;
    private float maxSpeed = 20f;

    //Rotational movement system
    //the amount of torque added by imaginary booster rockets (help get control back when spinning)using System.Collections;
    private float rotationalBoosters = 0.5f;
    //The number by which targetVector.x is divided within AddTorque()
    private float rotationalDamper = 30f;
    //now using to reset player on restart
    private float originalAngularDrag;
    //trying to get the position to reset on Restart()
    private Quaternion originalRotation;

    //Movement mouse input tracking - can be improved by using GetAxis("Mouse X"/"Mouse Y") I think 
    //takes into account where the mouse is when game starts, might be better to just have around the centre
    private Vector3 originalMousePosition;
    //was used in kinematic first attempt, just to keep z position under control
    private Vector3 targetPosition;
    //this is essentially THE vector to use with AddForce()
    private Vector3 targetVector;
    //is the targetVector set up and ready to be used via UpdateTargetVector()
    private bool bTargetV = false;
    //to convert the mouse position into an "identity matrix"? (is that where the value is between -1.0 and 1.0?) store the screen width and height
    private Vector3 screenModifier;
    //for damage which is used to kill the player
    private IStrengthManager strengthManager;
    //for laser power, removed by firing according to max-rounds setting in weapon
    private IStrengthManager powerManager;
    //our gun which is enabled by our PowerManager
    private LaserWeapon laser;
    public LaserWeapon Laser{
        get{return laser;}
    }

    void Start()
    {
        //I have made the centre point the centre of the screen so this is a little redundant
        SetupTargetVector();
    }

    //Use Awake to get all your references (instantiate objects) to avoid the dreaded NullReferenceExeption
    void Awake()
    {
        //connect with game manager - no need just use the Instance for GameOver()
        //gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //grab the strength manager which uses a slider as it's UI
        strengthManager = GetComponent<StrengthManager>();
        //and the power manager (which is also a StrengthManagerBase with different UI behaviour)
        powerManager = GetComponent<PowerManager>();
        //plug in our laser
        laser = GetComponent<LaserWeapon>();
        //Get the rigidbody so we can add our forces for a more natural game experience
        playerRB = GetComponent<Rigidbody>();

        //to reset on EnablePlayer() 
        originalRotation = transform.rotation;
        originalAngularDrag = playerRB.angularDrag;
    }

    // Update is called once per frame
    void Update()
    {
        //disable all behaviour if the player is dead
        if (!bIsPlaying) return;
        CheckInput();
    }

    void FixedUpdate()
    {
        if (!bIsPlaying) return;
        UpdateTargetVector();
        MoveMe(targetVector);
    }

    void LateUpdate()
    {
        if (!bIsPlaying) return;
        //this comes second as it does bounds checking which uses MoveMe() as well
        //UpdateTargetVector();
        //CheckForBounds();
    }


    void CheckInput()
    {
        //Keyboard based input ----
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Q to increase speed
            ChangeSpeed(1.0f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            // A to decrease speed
            ChangeSpeed(-1.0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }
        //Mouse button input...
        if (Input.GetMouseButtonDown(0))
        {//maybe these should be left and right mouse buttons
            // LEFT BUTTON for rotational correction boosters on the left side of ship (rotates clockwise)
            playerRB.AddTorque(Vector3.forward * rotationalBoosters * speed, ForceMode.Impulse);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // RIGHT BUTTON for rotational correction boosters on the right side of ship (rotates anti-clockwise)
            playerRB.AddTorque(Vector3.forward * -rotationalBoosters * speed, ForceMode.Impulse);
        }
    }

    public void DisablePlayer()
    {
        //the main boolean flag...
        bIsPlaying = false;
        //yep, so it can hide the text and the indicator
        strengthManager.Disable();
        powerManager.Disable();
        //this makes all of the physics stop so pause the game
        Time.timeScale = 0;
    }

    //Reset strength, power, speed, rotation and set bIsPlaying = true;
    public void EnablePlayer(float aStrength, float aPower)
    {
        playerRB.MoveRotation(originalRotation);
        //reset the strength managers to start value plus aStrength/aPower
        strengthManager.Enable();
        strengthManager.Reset();
        strengthManager.AddStrengthLevel(aStrength);

        powerManager.Enable();
        powerManager.Reset();
        //then add to the start power
        powerManager.AddStrengthLevel(aPower);
        speed = minSpeed;
        //Unpause the physics
        Time.timeScale = 1;
        bIsPlaying = true;
    }

    public void EnablePlayer()
    {
        EnablePlayer(0f, 0f);
    }

    // Behaviour when hit by collider with isTrigger = true
    private void OnTriggerEnter(Collider other)
    {
        //disable all behaviour if the player is dead
        if (!bIsPlaying) return;
        if (other.CompareTag("ChargeUp"))
        {
            //Charge up so add laser power
            powerManager.AddStrengthLevel(1);// AddPowerLevel(1);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("StrengthUp"))
        {
            //add a bit of strenth
            strengthManager.AddStrengthLevel(strengthManager.maxStrength / 10f);
            Destroy(other.gameObject);
        }
    }
    private void FireLaser()
    {
        //check that we have power to fire and that we're not already firing (rounds per second control)
        if (powerManager.strength > 0 && laser.Fire())
        {
            //if so take away from our ammunition
            powerManager.AddDamageLevel(powerManager.maxStrength / laser.maxRounds);
        }
    }

    //Behaviour when hit by Rigidbody with isTrigger = false in collider
    private void OnCollisionEnter(Collision other)
    {
        //disable all behaviour if the player is dead
        if (!bIsPlaying) return;
        // Non collider collision behaviour here
        // These collisions cause damage based on the mass of other
        if (other.gameObject.CompareTag("Planet") || other.gameObject.CompareTag("Meteor") || other.gameObject.CompareTag("Star"))
        {
            // You've bumped into a planet
            GameObject otherGO = other.gameObject;
            //if AddDamageLevel makes strength below zero it returns false, so then call GameOver()
            if (!strengthManager.AddDamageLevel(CalculateRbDamage(otherGO))) GameManager.Instance.GameOver();
        }
    }

    //use speed and rigidbody mass to calculate the damage level due to a collision
    float CalculateRbDamage(GameObject otherGO)
    {
        //How big was the hit? Basic sum based on mass and speed
        //using polymorphism with MoveForwardRb which is the base moving class
        float damage = speed + otherGO.GetComponent<MoveForwardRb>().speed;
        damage += otherGO.GetComponent<Rigidbody>().mass - playerRB.mass;
        return damage;
    }

    //the mouse motion control set up
    void SetupTargetVector()
    {
        screenModifier.x = Screen.width;
        screenModifier.y = Screen.height;
        //this is now the centre of the screen as it made more playability sense
        originalMousePosition = screenModifier / 2;//Input.mousePosition;
        targetPosition.z = transform.position.z;
        bTargetV = true;
    }

    //Modifies targetVector according to mouse position
    void UpdateTargetVector()
    {
        if (!bTargetV)
        {
            Debug.LogError("UpdateTargetVector cannot be called without first calling SetupTargetVector");
            return;
        }
        //first take away where the mouse was at the start so it's just about where it is in relation 
        targetVector = Input.mousePosition - originalMousePosition;
        //make the movement normalised (0.0-1.0) by dividing it by the screen size
        targetVector.x /= screenModifier.x;
        targetVector.y /= screenModifier.y;
        //reverse the x-axis
        targetVector.x = -targetVector.x;
        targetVector.z = targetPosition.z - transform.position.z;
        //multiply by speed so as you go faster the motion gets faster
        targetVector *= speed;
        //Let's try a different way - not working....
        // targetVector = Input.mousePosition;
        // targetVector.z = Camera.main.nearClipPlane;//targetPosition.z - transform.position.z;
        // //targetVector = Camera.main.ScreenToWorldPoint(targetVector);
        // Vector3.Normalize(targetVector);
        //Let's see if this happens to work first time...yeah :)
        //targetVector is modified in this function, makes it bounce off the bounds
        GameBounds.Instance.CheckForXYBounds(transform.position, ref targetVector);
    }

    void MoveMe(Vector3 target)
    {
        //going to add some rotational behaviour...
        playerRB.AddTorque(Vector3.forward * -((target.x*DifficultyManager.difficulty) / rotationalDamper), ForceMode.Impulse);
        playerRB.AddForce(target * speed, ForceMode.Impulse);
    }

    //for other game objects to access but not effect
    //public float GetSpeed(){return speed;}
    //implement as Properties now I've discovered that's how
    //c# deals with encapsulation

    void ChangeSpeed(float toChangeBy)
    {
        speed += toChangeBy;
        if (speed > maxSpeed) speed = maxSpeed;
        if (speed < minSpeed) speed = minSpeed;
        Debug.Log("Speed is: " + speed);
    }
}
