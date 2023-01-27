using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//bits that I'm trying
using GilesStrengthSystem;
using GilesWeapons;
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
    //trying to use this to produce a gui for the speed as well as allowing range to be set in Editor
    private IStrengthManager speedManager;
    private float speed = 10f;
    public float Speed{
        //with changes to MoveForwardRb this needs tweaking
        get{return 5 + speedManager.strength;}
    }
    
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
        //grab the strength managers which uses a slider as it's UI
        //with multiple strength managers they appear in the array in the order they are in the inspector
        StrengthManager[] strengthManagers = GetComponents<StrengthManager>();
        //this is a problem with my entire design now that I want to add a speed UI as another Strength Manager :/
        strengthManager = strengthManagers[0];
        speedManager = strengthManagers[1];
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
        {
            // LEFT BUTTON for rotational correction boosters on the left side of ship (rotates clockwise)
            playerRB.AddTorque(Vector3.forward * rotationalBoosters * Speed, ForceMode.Impulse);
        }
        if (Input.GetMouseButtonDown(1))
        {
            // RIGHT BUTTON for rotational correction boosters on the right side of ship (rotates anti-clockwise)
            playerRB.AddTorque(Vector3.forward * -rotationalBoosters * Speed, ForceMode.Impulse);
        }
    }

    public void DisablePlayer()
    {
        //the main boolean flag to false
        bIsPlaying = false;
        //yep, so it can hide the text and the indicator
        strengthManager.Disable();
        speedManager.Disable();
        powerManager.Disable();
        //this makes all of the physics stop so pause the game
        Time.timeScale = 0;
    }

    //Reset strength, power, speed, rotation and set bIsPlaying = true;
    public void EnablePlayer(float aStrength, float aPower)
    {
        EnablePlayer();
        strengthManager.AddStrengthLevel(aStrength);
        powerManager.AddStrengthLevel(aPower);
    }

    public void EnablePlayer()
    {        
        transform.rotation = originalRotation;
        //reset the strength manager
        strengthManager.Enable();
        strengthManager.Reset();

        //introducing the speedManager
        speedManager.Enable();
        speedManager.Reset();
        //going to externalise the speed so other things don't need a reference to the player
        GameProperties.Instance.SetGameProperty<int>(TagManager.PLAYER_SPEED_PROPERTY,(int)speedManager.strength);
        
        //and the ammunition
        powerManager.Enable();
        powerManager.Reset();

        //Unpause the physics
        Time.timeScale = 1;
        bIsPlaying = true;
    }

    // Behaviour when hit by collider with isTrigger = true
    private void OnTriggerEnter(Collider other)
    {
        //disable all behaviour if the player is dead
        if (!bIsPlaying) return;
        if (other.CompareTag(TagManager.AMMOBONUS))
        {
            //Charge up so add laser power
            powerManager.AddStrengthLevel(1);// AddPowerLevel(1);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag(TagManager.HEALTHBONUS))
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
        if (other.gameObject.CompareTag(TagManager.ENEMY1) || other.gameObject.CompareTag(TagManager.ENEMY2) || other.gameObject.CompareTag(TagManager.ENEMY3))
        {
            // You've bumped into an Enemy
            GameObject otherGO = other.gameObject;
            //if AddDamageLevel makes strength below zero it returns false, so then call GameOver()
            if (!strengthManager.AddDamageLevel(CalculateRbDamage(otherGO))) GameManager.Instance.GameOver();
        }
    }

    //use speed and rigidbody mass to calculate the damage level due to a collision
    float CalculateRbDamage(GameObject otherGO)
    {
        //How big was the hit? Basic sum based on mass and speed
        //using polymorphism? with MoveForwardRb which is the base moving class
        float damage = Speed + otherGO.GetComponent<MoveForwardRb>().speed;
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
        targetVector *= Speed;
        //Let's see if this happens to work first time...yeah :)
        //!! targetVector is modified in this function, makes it bounce off the bounds
        GameBounds.Instance.CheckForXYBounds(transform.position, ref targetVector);
    }

    void MoveMe(Vector3 target)
    {
        //going to add some rotational behaviour...
        playerRB.AddTorque(Vector3.forward * -((target.x*DifficultyManager.difficulty) / rotationalDamper), ForceMode.Impulse);
        playerRB.AddForce(target * Speed, ForceMode.Impulse);
    }

    //for other game objects to access but not effect
    //public float GetSpeed(){return speed;}
    //implement as Properties now I've discovered that's how
    //c# deals with encapsulation

    void ChangeSpeed(float toChangeBy)
    {
        if(toChangeBy > 0) speedManager.AddStrengthLevel(toChangeBy);
        else speedManager.AddDamageLevel(toChangeBy*-1);
        //going to externalise the speed so other things don't need a reference to the player
        GameProperties.Instance.SetGameProperty<int>(TagManager.PLAYER_SPEED_PROPERTY,(int)speedManager.strength);
        Debug.Log("Speed is: " + GameProperties.Instance.GetGameProperty<int>("Player Speed"));
    }
}
