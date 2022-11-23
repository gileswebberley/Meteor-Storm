using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GilesManagers{
public class StrengthManager : MonoBehaviour
{
    //Implement damage/strength
    private float _strength = 500f;
    public float strength {
        get {return _strength;}
    }
    //strength set by artist saved for resetting
    [SerializeField] private float _startStrength = 500f;
    public float startStrength {
        get {return _startStrength;}
    }
    //set by artist and used for strength slider UI
    [SerializeField] private float _maxStrength = 1000f;
    public float maxStrength {
        get {return _maxStrength;}
    }
    //the UnityEngine.UI component set in Editor
    [SerializeField] private Slider strengthSlider;
    //the fill component of the strength slider UI
    private GameObject sliderFill;
    //game object container for strength UI set in Editor
    [SerializeField] private GameObject strengthUIArea;

    //I was going to implement a listener for game over but instead decided to
    //return a boolean from AddDamageLevel() which can be examined for death

    public void Enable()
    {
       EnableUI();
        _strength = 0;
    }

    public void Disable()
    {        
        //strengthUIArea.SetActive(false);
        DisableUI();
    }

    public virtual void EnableUI()
    {
        //strengthUIArea = GameObject.Find("Strength").gameObject;
        //Strength slider is attached in the Editor
        //get the fill of the strength slider so it can be updated
        sliderFill = strengthSlider.fillRect.gameObject;
        //set the slider parameters to match the strength
        strengthSlider.maxValue = maxStrength;
        //a test whether I've grabbed the correct game object
        //yep, so it can hide the text and the indicator
        strengthUIArea.SetActive(true);
    }

    public virtual void DisableUI()
    {        
        strengthUIArea.SetActive(false);
    }

    void Awake()
    {
        Disable();
    }

    //Implement damage which is based on the mass of what's hit you
    //when it get's to zero it's GAME OVER - to extract it out into a class
    public bool AddDamageLevel(float damage){
        //How big was the hit? Basic sum based on mass and speed
        //using polymorphism with MoveForwardRb which is the base moving class
        //float damage = speed + otherGO.GetComponent<MoveForwardRb>().speed + otherGO.GetComponent<Rigidbody>().mass - playerRB.mass;
        _strength -= damage;
        Debug.Log("DAMAGE: Strength is now: "+strength);

        //If we are completely damaged it's game over
        if(_strength <= 0){
            //GameOver calls DisablePlayer()
            _strength = 0;
            return false;
            //gameHQ.GameOver();
        }
        UpdateUI();
        return true;
    }

    //the opposite of AddDamageLevel()
    public void AddStrengthLevel(float toAdd){
        if(toAdd < 0){
            Debug.LogError("No Negative values allowed in AddStrengthLevel() Please use AddDamageLevel() to \"remove strength\"");
        }
        else{
            _strength += toAdd;
            //maxes out at it's start value
            if(strength >= maxStrength){
                _strength = maxStrength;
            } 
            UpdateUI();
        }
    }

    protected virtual void UpdateUI()
    {
        sliderFill.SetActive(true);
        strengthSlider.value = strength;
        Debug.Log("ADD: Strength is now: "+strength);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
