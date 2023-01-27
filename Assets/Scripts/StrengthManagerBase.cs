using UnityEngine;
using GilesSpawnSystem;

namespace GilesStrengthSystem
{
    public class StrengthManagerBase : MonoBehaviour, IStrengthManager
    {
        //Implement damage/strength
        protected float _strength = 500f;
        public float strength
        {
            get { return _strength; }
            protected set { _strength = value; }
        }
        //strength set by artist saved for resetting
        [SerializeField] private float _startStrength = 500f;
        public float startStrength
        {
            get { return _startStrength; }
        }
        //set by artist and used for strength slider UI
        [SerializeField] protected float _maxStrength = 1000f;
        public float maxStrength
        {
            get { return _maxStrength; }
        }

        
        //set by artist and used for strength slider UI
        [SerializeField] protected float _minStrength = 0f;
        public float minStrength
        {
            get { return _minStrength; }
        }

        //I was going to implement a listener for game over but instead decided to
        //return a boolean from AddDamageLevel() which can be examined for death

        public virtual void Enable()
        {
            EnableUI();
        }

        public virtual void Reset()
        {
             _strength = _startStrength;
             UpdateUI();
        }

        public virtual void Disable()
        {
            //strengthUIArea.SetActive(false);
            DisableUI();
        }

        public virtual void EnableUI(){}

        public virtual void DisableUI(){}

        void Awake()
        {
            Disable();
        }

        //Implement damage which is based on the mass of what's hit you
        //when it get's to zero it's GAME OVER - to extract it out into a class
        public virtual bool AddDamageLevel(float damage)
        {
            _strength -= damage;
            Debug.Log("DAMAGE: Strength is now: " + _strength);

            //If we are completely damaged it's game over
            if (_strength <= _minStrength)
            {
                _strength = _minStrength;
                return false;
                //this functionality comes from returning the bool above so game manager keeps control of state
                //gameHQ.GameOver();
            }
            UpdateUI();
            return true;
        }

        //the opposite of AddDamageLevel()
        public virtual void AddStrengthLevel(float toAdd)
        {
            if (toAdd < 0)
            {
                Debug.LogError("No Negative values allowed in AddStrengthLevel() Please use AddDamageLevel() to \"remove strength\"");
            }
            else
            {
                _strength += toAdd;
                //maxes out at it's max value
                if (strength >= maxStrength)
                {
                    _strength = maxStrength;
                }
                UpdateUI();
            }
        }

        public virtual void UpdateUI()
        {
            Debug.Log("NO UI AVAILABLE FOR UpdateUI(): Strength is now: " + _strength);
        }
    }
}
