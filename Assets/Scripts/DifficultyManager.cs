using System.Collections;
using UnityEngine;

//custom namespace created for my attempts at refactoring Meteor Storm
namespace OodModels
{
    //break out functionality from GameManager, MonoBehaviour is for Start/StopCoroutine()
    //only want one per game I think, perhaps call it StaticDifficultyManager 
    public class DifficultyManager
    {
        //Create the private singleton instance - can't new MonoBehaviour() hence the _mono solution
        private static DifficultyManager _instance; 
        // //Property to encapsulate the _instance
        // //only needed to access non-static members
        public static DifficultyManager Instance
        {
            get {if(_instance != null) return _instance;
                else _instance = new DifficultyManager();
                return _instance; }
        }
        // //Private constructor
        private DifficultyManager() { }

        //introducing a monobehaviour object reference to run coroutines
        //decided that for just this functionality it wasn't worth inheriting
        //and also trying to learn how to work with C# so good experimentation
        private static MonoBehaviour _mono;
        public static MonoBehaviour mono {
            //we just want to be able to use it for ourselves so protected access
            protected get {return _mono;}
            //but we want any MonoBehaviour to be able to set itself for us to use
            set {_mono = value;}
        }

        //The difficulty properties themselves
        private static int _maxDifficulty = 5;
        public static int maxDifficulty
        {
            get { return _maxDifficulty; }
            set { Instance.SetMaxDifficulty(value); }
        }
        private static int _difficulty = 1;
        public static int difficulty
        {
            get { return _difficulty; }
            set { Instance.SetDifficulty(value); }
        }

        //The auto difficulty change system, this is why I have the MonoBehaviour reference
        //so I can use it's coroutine system as I am new to the concept in C#
        private float difficultyChangeTime;

        public void StartDifficultyStepTimer(float stepSeconds)
        {
            if(_mono != null){
                //in case it's already been started from elsewhere
                _mono.StopCoroutine(DifficultyChangeTimer());
                difficultyChangeTime = stepSeconds;
                _mono.StartCoroutine(DifficultyChangeTimer());
            }else{
                Debug.LogError("DifficultyManager._mono is null");
            }
        }

        public void StopDifficultyStepTimer()
        {
            if(_mono != null){
                _mono.StopCoroutine(DifficultyChangeTimer());
            }else{
                Debug.LogError("DifficultyManager._mono is null");
            }
        }

        IEnumerator DifficultyChangeTimer()
        {
            Debug.Log("DifficultyChangeTimer waiting....");
            yield return new WaitForSeconds(difficultyChangeTime);
            Debug.Log("DifficultyChangeTimer finished waiting");
            //then add 1 to difficulty
            if (IncrementDifficulty())
            {
                Debug.Log("DifficultyChangeTimer incrementing");
                //if not topped out, resursively call this iterator
                _mono.StartCoroutine(DifficultyChangeTimer());
            }
        }

        //the setters which check the bounds and clamp the values
        public void SetDifficulty(int d)
        {
            if(d == _difficulty) return;
            Debug.Log($"SetDifficulty({d})");
            if (d > _maxDifficulty)
            {
                _difficulty = _maxDifficulty;
            }
            else if (d < 1)
            {
                _difficulty = 1;
            }
            else
            {
                _difficulty = d;
            }

        }

        public void SetMaxDifficulty(int d)
        {
            if(d == _maxDifficulty) return;
            Debug.Log($"SetMaxDifficulty({d})");
            if (d < 1)
            {
                _maxDifficulty = 1;
            }
            else
            {
                _maxDifficulty = d;
            }
        }

        //It might be nice to look into some kind of operator overloading here if that's possible
        //return false if already at max difficulty
        public bool IncrementDifficulty()
        {
            if (_difficulty >= _maxDifficulty)
            {
                return false;
            }
            else
            {
                _difficulty++;
                Debug.Log($"Increment : difiiculty is now: {_difficulty}");
                return true;
            }
        }

        //returns false if already at 1
        public bool DecrementDifficulty()
        {
            if (_difficulty <= 1)
            {
                return false;
            }
            else
            {
                //if safe change the value
                _difficulty--;
                Debug.Log($"Decrement : difiiculty is now: {_difficulty}");
                return true;
            }
        }
    }
}
