using System.Collections;
using UnityEngine;

//custom namespace created for my attempts at refactoring Meteor Storm
namespace OodModels
{
    //break out functionality from GameManager, MonoBehaviour is for Start/StopCoroutine()
    public class DifficultyManager
    {
        //Create the private singleton instance - can't with MOnoBehaviour :(
        private static readonly DifficultyManager _instance = new DifficultyManager();
        // //Private constructor
        private DifficultyManager() { }
        //introducing a monobehaviour object reference to run coroutines
        private static MonoBehaviour _mono = null;
        public static MonoBehaviour mono {
            protected get {return _mono;}
            set {_mono = value;}
        }
        // //Property to encapsulate the _instance
        // //only needed to access non-static members
        public static DifficultyManager Instance
        {
            get { return _instance; }
        }
        private static int _maxDifficulty = 5;
        public static int maxDifficulty
        {
            get { return _maxDifficulty; }
            set { _instance.SetMaxDifficulty(value); }
        }
        private static int _difficulty = 1;
        public static int difficulty
        {
            get { return _difficulty; }
            set { _instance.SetDifficulty(value); }
        }

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
            Debug.Log("DifficultyChangeTimer");
            yield return new WaitForSeconds(difficultyChangeTime);
            //then add 1 to difficulty
            if (IncrementDifficulty())
            {
                //if not topped out resursively call this iterator
                _mono.StartCoroutine(DifficultyChangeTimer());
            }
        }

        public void SetDifficulty(int d)
        {
            if (d > _maxDifficulty)
            {
                _difficulty = _maxDifficulty;
                return;
            }
            else if (d < 1)
            {
                _difficulty = 1;
                return;
            }
            else
            {
                _difficulty = d;
            }

        }

        public void SetMaxDifficulty(int d)
        {
            if (d < 1)
            {
                _maxDifficulty = 1;
                return;
            }
            else
            {
                _maxDifficulty = d;
            }
        }
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
                Debug.Log("Increment : difiiculty is now: "+_difficulty);
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
                return true;
            }
        }
    }
}
