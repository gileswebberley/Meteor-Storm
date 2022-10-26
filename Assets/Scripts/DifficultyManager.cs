using System.Collections;
using UnityEngine;

//custom namespace created for my attempts at refactoring Meteor Storm
namespace OodModels
{
    //break out functionality from GameManager, MonoBehaviour is for Start/StopCoroutine()
    public class DifficultyManager : MonoBehaviour
    {
        //Create the private singleton instance
        private static readonly DifficultyManager _instance = new DifficultyManager();
        //Private constructor
        private DifficultyManager() { }
        //Property to encapsulate the _instance
        //only needed to access non-static members
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
            //in case it's already been started from elsewhere
            StopCoroutine("DifficultyChangeTimer");
            difficultyChangeTime = stepSeconds;
            StartCoroutine("DifficultyChangeTimer");
        }

        public void StopDifficultyStepTimer()
        {
            StopCoroutine("DifficultyChangeTimer");
        }

        IEnumerator DifficultyChangeTimer()
        {
            yield return new WaitForSeconds(difficultyChangeTime);
            //then add 1 to difficulty
            if (IncrementDifficulty())
            {
                //if not topped out resursively call this iterator
                StartCoroutine("DifficultyChangeTimer");
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
