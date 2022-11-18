using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//for the GameBounds singleton
using OodModels;

public class PlayerController : MonoBehaviour
{
    public Slider strengthSlider;
    private GameObject sliderFill;
    private GameObject damageText;
    //very important boolean - for IPlayable
    private bool bIsPlaying = false;
    [SerializeField] private GameObject laser;

    private GameManager gameHQ;
    private Rigidbody playerRB;
    //takes into account where the mouse is when game starts, might be better to just have around the centre
    private Vector3 originalMousePosition;
    //was used in kinematic first attempt
    private Vector3 targetPosition;
    //this is essentially THE vector to use with AddForce()
    private Vector3 targetVector;
    //is the targetVector set up and ready to be used via UpdateTargetVector()
    private bool bTargetV = false;
    //to convert the mouse position into an "identity matrix"? (is that where the value is between -1.0 and 1.0?) store the screen width and height
    private Vector3 screenModifier;

    //deprecated for GameBounds use instead
    // //maximum devation across
    private float xBounds;
    // //maximum deviation up and down
    private float yBounds;

    //all the speed based properties -------
    //affects control twitchyness, other objects use it to make it feel like speed has changed
    private float speed = 10f;
    private float minSpeed = 5f;
    private float maxSpeed = 20f;
    //the amount of torque added by imaginary booster rockets (help get control back when spinning)using System.Collections;
    private float rotationalBoosters = 0.5f;
    //The number by which targetVector.x is divided within AddTorque()
    private float rotationalDamper = 30f;
    //now using to reset player on restart
    private float originalAngularDrag;

    private float power = 0f;
    [SerializeField] private float startPower = 10f;
    [SerializeField] private float maxPower = 20f;
    private float indicatorMoveStep;
    private GameObject powerIndicator;
    //how much power so the victims can check how much to remove from themselves
    private float laserPower = 1f;
    //to use up power when firing rather than limit the firing, a divisor of maxPower
    private float laserPowerUsageDivisor = 100f;//so number of shots with max power
    //originally had this as maxPower so it would be 1 when on full power
    private float laserPowerDivisor;
    private bool bIsFiring = false;
    private float roundsPerSecond = 10f;
    //Implement damage
    private float strength = 500f;
    [SerializeField] private float startStrength = 500f;
    [SerializeField] private float maxStrength = 1000f;

    //trying to get the position to reset on Restart()
    private Quaternion originalRotation;

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
        damageText = transform.Find("Strength").gameObject;
        damageText.SetActive(false);

        //Strength slider is attached in the Editor
        //get the fill of the strength slider so it can be updated
        sliderFill = strengthSlider.fillRect.gameObject;
        //set the divisor for lasers so on max power it has a value of 2 - deprecated for now (playability)
        laserPowerDivisor = maxPower/2f;
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
        //this comes second as it does bounds checking which uses MoveMe() as well
        UpdateTargetVector();
    }

    void LateUpdate(){
        
    }

    public void DisablePlayer(){
        bIsPlaying = false;
        //yep, so it can hide the text and the indicator
        damageText.SetActive(false);
        playerRB.angularDrag = originalAngularDrag + 20f;
    }

    public void EnablePlayer(float aStrength, float aPower){
        playerRB.MoveRotation(originalRotation);
        playerRB.angularDrag = originalAngularDrag;
        //set the slider parameters to match the strength
        //maxStrength = aStrength;
        strengthSlider.maxValue = maxStrength;
        //a test whether I've grabbed the correct game object
        //yep, so it can hide the text and the indicator
        damageText.SetActive(true);
        strength = 0;
        //use this so it looks after the UI component
        AddStrengthLevel(aStrength);
        speed = minSpeed;
        AddPowerLevel(-power);
        AddPowerLevel(aPower);
        bIsPlaying = true;
    }

    public void EnablePlayer()
    {
        EnablePlayer(startStrength,startPower);
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
            AddStrengthLevel(maxStrength/10f);
            Destroy(other.gameObject);
        }else if(other.CompareTag("Star")){
            //death by a star, damage is based on mass in rigidbody
            AddDamageLevel(other.gameObject);
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
            StartCoroutine("LimitShotsPerSecond");
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
            AddDamageLevel(other.gameObject);
        }
        if(other.gameObject.CompareTag("Meteor")){
            // You've bumped into a meteor
            AddDamageLevel(other.gameObject);
        }
    }

    //Implement damage which is based on the mass of what's hit you
    //when it get's to zero it's GAME OVER
    void AddDamageLevel(GameObject otherGO){
        //How big was the hit? Basic sum based on mass and speed
        //using polymorphism with MoveForwardRb which is the base moving class
        float damage = speed + otherGO.GetComponent<MoveForwardRb>().speed + otherGO.GetComponent<Rigidbody>().mass - playerRB.mass;
        strength -= damage;
        Debug.Log("DAMAGE: Strength is now: "+strength);

        //If we are completely damaged it's game over
        if(strength <= 0 && bIsPlaying){
            //GameOver calls DisablePlayer()
            gameHQ.GameOver();
            strength = 0;
        }
        sliderFill.SetActive(true);
        strengthSlider.value = strength;
    }
    //the opposite of AddDamageLevel()
    void AddStrengthLevel(float toAdd){
        if(toAdd < 0){
            Debug.LogError("Please use AddDamageLevel(gameobject) to remove strength");
        }
        else{
            strength += toAdd;
            //maxes out at it's start value
            if(strength >= maxStrength){
                strength = maxStrength;
            }            
            sliderFill.SetActive(true);
            strengthSlider.value = strength;
            Debug.Log("ADD: Strength is now: "+strength);
        }
    }

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
