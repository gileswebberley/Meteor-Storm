using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//for the GameBounds singleton
using OodModels;
using GilesManagers;

[RequireComponent(typeof(StrengthManager))]
public class PlayerController : MonoBehaviour
{    
    //Implement damage/strength
    // private float strength = 500f;
    // //strength set by artist saved for resetting
    // [SerializeField] private float startStrength = 500f;
    // //set by artist and used for strength slider UI
    // [SerializeField] private float maxStrength = 1000f;
    // //the UnityEngine.UI component
    // public Slider strengthSlider;
    // //the fill component of the strength slider UI
    // private GameObject sliderFill;
    // //game object container for strength UI
    // private GameObject strengthUIArea;

    //very important boolean - for IPlayable me thinks
    private bool bIsPlaying = false;

    //Weaponisation related items
    private float power = 0f;
    [SerializeField] private float startPower = 10f;
    [SerializeField] private float maxPower = 20f;
    //the laser power related UI - should perhaps be part of a IWeapon?
    private GameObject powerIndicator;
    //amount to move the sliding component of the UI above
    private float indicatorMoveStep;
    //how much power each round has so the victims can check how much to remove from themselves
    private float laserPower = 1f;
    //number of rounds per full power load (so number of shots with max power)
    private float laserPowerUsageDivisor = 100f;
    //originally had this as maxPower so it would be 1 when on full power - deprecated
    //private float laserPowerDivisor;
    //lock used for Firing input control
    private bool bIsFiring = false;
    //time step control for Firing input
    private float roundsPerSecond = 10f;
    //the game object to be "fired"
    [SerializeField] private GameObject laser;

    private GameManager gameHQ;

    //Movement related control system
    private Rigidbody playerRB;
    //all the speed based properties -------
    //affects control twitchyness, other objects use it to make it feel like speed has changed
    private float speed = 10f;
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

    //deprecated for GameBounds use instead
    // //maximum devation across
    //private float xBounds;
    // //maximum deviation up and down
    //private float yBounds;
    private StrengthManager strengthManager;

    // Start is called before the first frame update
    void Start()
    {     
        //I have made the centre point the centre of the screen so this is a little redundant
        SetupTargetVector();
    }

    //Use Awake to get all your references (instantiate objects) to avoid the dreaded NullReferenceExeption
    void Awake(){
        //use transform.Find for children game objects
        powerIndicator = transform.Find("Power Level").gameObject;
        //the float is the distance to move the power cylinder forwards to create the nice effect
        indicatorMoveStep = -0.5f / maxPower;
        //connect with game manager
        gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //grab the strength manager
        strengthManager = GetComponent<StrengthManager>();
        // strengthManager.Enable();
        // strengthUIArea = transform.Find("Strength").gameObject;
        // strengthUIArea.SetActive(false);

        //Strength slider is attached in the Editor
        //get the fill of the strength slider so it can be updated
        // sliderFill = strengthSlider.fillRect.gameObject;
        //set the divisor for lasers so on max power it has a value of 2 - deprecated for now (playability)
        //laserPowerDivisor = maxPower/2f;
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
        if(!bIsPlaying) return;        
        CheckInput();
    }

    void CheckInput(){        
        //Keyboard based input ----
        if(Input.GetKeyDown(KeyCode.Q)){
            // Q to increase speed
            ChangeSpeed(1.0f);
        }
        if(Input.GetKeyDown(KeyCode.A)){
            // A to decrease speed
            ChangeSpeed(-1.0f);
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            FireLaser();
        }
        //Mouse button input...
        if(Input.GetMouseButtonDown(0)){//maybe these should be left and right mouse buttons
            // LEFT BUTTON for rotational correction boosters on the left side of ship (rotates clockwise)
            playerRB.AddTorque(Vector3.forward * rotationalBoosters * speed,ForceMode.Impulse);
        }
        if(Input.GetMouseButtonDown(1)){
            // RIGHT BUTTON for rotational correction boosters on the right side of ship (rotates anti-clockwise)
            playerRB.AddTorque(Vector3.forward * -rotationalBoosters * speed,ForceMode.Impulse);
        }
    }

    void FixedUpdate(){
        if(!bIsPlaying) return; 
        MoveMe(targetVector);
    }

    void LateUpdate(){
        if(!bIsPlaying) return;
        //this comes second as it does bounds checking which uses MoveMe() as well
        UpdateTargetVector();
    }

    public void DisablePlayer(){
        bIsPlaying = false;
        //yep, so it can hide the text and the indicator
        strengthManager.Disable();
        playerRB.angularDrag = originalAngularDrag + 20f;
    }

    public void EnablePlayer(float aStrength, float aPower){
        playerRB.MoveRotation(originalRotation);
        playerRB.angularDrag = originalAngularDrag;

        strengthManager.Enable();
        //set the slider parameters to match the strength
        // strengthSlider.maxValue = maxStrength;
        // //a test whether I've grabbed the correct game object
        // //yep, so it can hide the text and the indicator
        // strengthUIArea.SetActive(true);
        // strength = 0;
        //use this so it looks after the UI component
        strengthManager.AddStrengthLevel(aStrength);
        speed = minSpeed;
        AddPowerLevel(-power);
        AddPowerLevel(aPower);
        bIsPlaying = true;
    }

    public void EnablePlayer()
    {
        EnablePlayer(strengthManager.startStrength,startPower);
    }

    // Behaviour when hit by collider with isTrigger = true
    private void OnTriggerEnter(Collider other){
        //disable all behaviour if the player is dead
        if(!bIsPlaying)return;
        if(other.CompareTag("ChargeUp")){
            //Charge up so add laser power
            AddPowerLevel(1);
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("StrengthUp")){
            //add a bit of strenth
            strengthManager.AddStrengthLevel(strengthManager.maxStrength/10f);
            Destroy(other.gameObject);
        }else if(other.CompareTag("Star")){
            //death by a star, damage is based on mass in rigidbody
            GameObject otherGO = other.gameObject;
            strengthManager.AddDamageLevel(CalculateRbDamage(otherGO));
        }
    }

    private void AddPowerLevel(float toAdd){
        power += toAdd;
        if(power <= 0){
            //run out of power
            power = 0;           
        }
        else if(power > maxPower){
            power = maxPower;
        }

        UpdatePowerIndicator();
    }

    private void UpdatePowerIndicator(){
        //Frustratingly it took me ages to realise that I had to use localPosition
        powerIndicator.transform.localPosition = new Vector3(0.0f,indicatorMoveStep*power,0.0f);
    }

    private void FireLaser(){
        //++ LimitShotsPerSecond() introduced

        //lasers are at power level 1 when fully powered up
        //going to remove this as it makes it too frustrating
        //laserPower = power/laserPowerDivisor;

        //check that we have power to fire
        if(laserPower > 0 && !bIsFiring){
            bIsFiring = true;
            Vector3 gunPosition = new Vector3(transform.position.x,transform.position.y+1, transform.position.z-3);
            GameObject shot = Instantiate(laser,gunPosition,transform.rotation);
            //++ I want to have the lasers be affected by our current direction to add to playability
            shot.GetComponent<Rigidbody>().AddForce(targetVector*speed/2,ForceMode.Impulse);
            //lasers use the power
            AddPowerLevel(-(maxPower/laserPowerUsageDivisor));
            //pause trigger based on roundsPerSecond
            StartCoroutine(LimitShotsPerSecond());
        }
    }

    IEnumerator LimitShotsPerSecond(){
        yield return new WaitForSeconds(1/roundsPerSecond);
        bIsFiring = false;
    }
    public float GetLaserPower(){
        return laserPower;
    }

    //Behaviour when hit by Rigidbody with isTrigger = false in collider
    private void OnCollisionEnter(Collision other){
        //disable all behaviour if the player is dead
        if(!bIsPlaying)return;
        // Non collider collision behaviour here
        // These collisions cause damage based on the mass of other
        if(other.gameObject.CompareTag("Planet")){//why are these if statements here?
            // You've bumped into a planet
            GameObject otherGO = other.gameObject;
            strengthManager.AddDamageLevel(CalculateRbDamage(otherGO));
        }
        if(other.gameObject.CompareTag("Meteor")){
            // You've bumped into a meteor
            GameObject otherGO = other.gameObject;
            strengthManager.AddDamageLevel(CalculateRbDamage(otherGO));
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

    //Implement damage which is based on the mass of what's hit you
    //when it get's to zero it's GAME OVER - to extract it out into a class
    // void AddDamageLevel(float damage){
    //     //How big was the hit? Basic sum based on mass and speed
    //     //using polymorphism with MoveForwardRb which is the base moving class
    //     //float damage = speed + otherGO.GetComponent<MoveForwardRb>().speed + otherGO.GetComponent<Rigidbody>().mass - playerRB.mass;
    //     strength -= damage;
    //     Debug.Log("DAMAGE: Strength is now: "+strength);

    //     //If we are completely damaged it's game over
    //     if(strength <= 0 && bIsPlaying){
    //         //GameOver calls DisablePlayer()
    //         strength = 0;
    //         gameHQ.GameOver();
    //     }
    //     UpdateStrengthIndicator();
    // }
    //the opposite of AddDamageLevel()
    // void AddStrengthLevel(float toAdd){
    //     if(toAdd < 0){
    //         Debug.LogError("No Negative values allowed in AddStrengthLevel() Please use AddDamageLevel() to \"remove strength\"");
    //     }
    //     else{
    //         strength += toAdd;
    //         //maxes out at it's start value
    //         if(strength >= maxStrength){
    //             strength = maxStrength;
    //         } 
    //         UpdateStrengthIndicator();
    //     }
    // }

    // void UpdateStrengthIndicator()
    // {
    //     sliderFill.SetActive(true);
    //     strengthSlider.value = strength;
    //     Debug.Log("ADD: Strength is now: "+strength);
    // }

    //returns multiplier for targetVector [bInXBounds,bInYBounds,1] and pushes gameObject back into bounds
    Vector3 CheckForBounds(){
        Vector3 temp = new Vector3(1,1,1);
        //gone too wide so push us back into the middle and return [0,y,1] 
        if(transform.position.x + targetVector.x > GameBounds.maxX){
            temp.x = 0;
            MoveMe(Vector3.left);
            //playerRB.AddForce(Vector3.left*speed,ForceMode.Impulse);
        }else if(transform.position.x + targetVector.x < GameBounds.minX){            
            temp.x = 0;
            MoveMe(Vector3.right);
            //playerRB.AddForce(Vector3.right*speed,ForceMode.Impulse);
        }
        //gone too high or low so push us back into the middle and return[x,0,1]
        if(transform.position.y + targetVector.y > GameBounds.maxY){
            temp.y = 0;
            MoveMe(Vector3.down);
            //playerRB.AddForce(Vector3.down,ForceMode.Impulse);
        }else if(transform.position.y + targetVector.y < GameBounds.minY){            
            temp.y = 0;
            MoveMe(Vector3.up);
            //playerRB.AddForce(Vector3.up,ForceMode.Impulse);
        }
        return temp;
    }

    //the mouse motion control set up
    void SetupTargetVector(){ 
        screenModifier.x = Screen.width;
        screenModifier.y = Screen.height;
        //this is now the centre of the screen as it made more playability sense
        originalMousePosition = screenModifier/2;//Input.mousePosition;
        targetPosition.z = transform.position.z;
        bTargetV = true;
    }

    //Modifies targetVector according to mouse position
    void UpdateTargetVector(){
        if(!bTargetV){
            Debug.LogError("UpdateTargetVector cannot be called without first calling SetupTargetVector");
            return;
        }
        //first take away where the mouse was at the start so it's just about where it is in relation 
        targetVector = Input.mousePosition - originalMousePosition;
        //make the movement a float (0.0-1.0) by dividing it by the screen size
        targetVector.x /= screenModifier.x;
        targetVector.y /= screenModifier.y;
        //reverse the x-axis
        targetVector.x = -targetVector.x;
        targetVector.z = targetPosition.z - transform.position.z;
        //multiply by speed so as you go faster the motion gets more twitchy
        targetVector *= speed;
        //check that we aren't going out of bounds and correct appropriately
        //this is in this method so that MoveMe() could stay pure, it's used in the bounds check
        //so would loop around pointlessly if moved to MoveMe() - don't do it!
        Vector3 boundsCheck = CheckForBounds();
        targetVector = Vector3.Scale(targetVector, boundsCheck);        
    }

    void MoveMe(Vector3 target){
        playerRB.AddTorque(Vector3.forward * -((target.x*speed)/rotationalDamper),ForceMode.Impulse);
        playerRB.AddForce(target*speed,ForceMode.Impulse);
    }

    //for other game objects to access but not effect
    //implement as Properties now I've discovered that's how
    //c# deals with encapsulation
    public float GetSpeed(){
        return speed;
    }

    public float GetMaxSpeed(){
        return maxSpeed;
    }

    void ChangeSpeed(float n){
        speed += n;
        if(speed > maxSpeed) speed = maxSpeed;
        if(speed < minSpeed) speed = minSpeed;
        Debug.Log("Speed is: "+speed);
    }
}
