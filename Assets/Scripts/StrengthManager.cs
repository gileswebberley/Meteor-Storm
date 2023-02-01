using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GilesSpawnSystem;

namespace GilesStrengthSystem
{
public class StrengthManager : StrengthManagerBase, IStrengthManager
{
    //the UnityEngine.UI component set in Editor
    [SerializeField] private Slider strengthSlider;
    //the fill component of the strength slider UI
    private GameObject sliderFill;
    //game object container for strength UI set in Editor
    [SerializeField] private GameObject strengthUIArea;

    public override void EnableUI()
    {
        //Strength slider is attached in the Editor
        //get the fill of the strength slider so it can be updated
        sliderFill = strengthSlider.fillRect.gameObject;
        //set the slider parameters to match the strength
        strengthSlider.maxValue = _maxStrength;
        strengthSlider.minValue = _minStrength;
        UpdateUI();
        //a test whether I've grabbed the correct game object
        //yep, so it can hide the text and the indicator
        strengthUIArea.SetActive(true);
    }

    public override void DisableUI()
    {
        strengthUIArea.SetActive(false);
    }

    public override void UpdateUI()
    {
        sliderFill.SetActive(true);
        strengthSlider.value = strength;
        Debug.Log("ADD: Strength is now: " + _strength);
    }
}
}
