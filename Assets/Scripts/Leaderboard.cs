using System.Collections.Generic;
using UnityEngine;

namespace OodModels
{
    //This was originally ScoreManager! realised that this is something to use WITH scores not FOR them
    [System.Serializable]
    public abstract class Leaderboard
    {
        //make the leaderboard itself large and then use leaderboardSize for what we work with? 
        [SerializeField] protected List<ScoreData> leaderboardArray = new List<ScoreData>(100);

        //default constructor
        public Leaderboard()
        {
            leaderboardArray.Capacity = _leaderboardCapacity;
        }

        //just so we keep control of the size of memory used
        private int _leaderboardCapacity = 100;
        public int leaderboardCapacity {
            get {return _leaderboardCapacity;}
            set {
                _leaderboardCapacity = value;
                leaderboardArray.Capacity = _leaderboardCapacity;
                }//this could be dangerous if threaded?
        }
        //private because changing has an effect on the internal List
        private int _leaderboardSize = 10;
        public int leaderboardSize
        {
            get { return _leaderboardSize; }
            set { ChangeLeaderboardSize(value); }
        }

        //want to abstract out the way it's stored, make it an abstract class
        protected string _leaderboardName = "Leaderboard-Default";
        public string leaderboardName
        {
            get { return _leaderboardName; }
            set { _leaderboardName = value; }
        }

        //Depending on how it's being stored use the leaderboardName
        //and deal with loading and saving in derived class
        //the AddToLeaderboardRaw is defined in here for use by these
        //derived classes
        public abstract void AddToLeaderboard(string name, int score);
        public abstract void AddToLeaderboard(ScoreData newScore);

        //the derived class method of loading using name
        public abstract void LoadLeaderboard(string name);
        //the derived class method of saving using name
        public abstract void SaveLeaderboard(string name);
        //the derived class method of loading using _leaderboardName
        //return success
        public abstract bool LoadLeaderboard();
        //the derived class method of saving using _leaderboardName
        public abstract bool SaveLeaderboard();

        //Provided in here as it is only dealing with the underlying data
        protected void AddToLeaderboardRaw(ScoreData newScore)
        {
            if (leaderboardArray != null)
            {
                //--------- extract out
                leaderboardArray.Add(newScore);
                //then sort based on the ScoreData Comparable I hope - doesn't seem to be working
                leaderboardArray.Sort();

                Debug.Log($"New score (rank): {newScore.name} : {newScore.score} ({leaderboardArray.IndexOf(newScore)+1}) added to the {_leaderboardName} leaderboard");
                //if we're about to resize the list clip off the last entry to enforce capacity
                if (leaderboardArray.Count >= leaderboardArray.Capacity - 1)
                {
                    //then get rid of the lowest score
                    leaderboardArray.RemoveAt(leaderboardArray.Count);
                }
                //-----------to here
            }else{
                Debug.LogError("Leaderboard is NULL in AddToLeaderboard()");
            }
        }

        protected virtual List<ScoreData> GetLeaderboardRaw(){
            List<ScoreData> temp = new List<ScoreData>(_leaderboardSize);//use GetRange(0,_leaderboardSize)?
            for(int i = 0; i < _leaderboardSize; i++){
                //this is where it fails as leaderboardArray has only saved as {}, although ScoreData serialises properly
                Debug.Log($"i: {i} Leaderboard size: {leaderboardArray.Count}");
                temp.Add(leaderboardArray[i]);
            }  
            //List<ScoreData> temp = leaderboardArray.GetRange(0,_leaderboardSize);
            return temp;
        }

        private void ChangeLeaderboardSize(int s)
        {
            //keep the array tidy
            if (s > leaderboardArray.Capacity)
            {
                //it's bigger than the leaderboardArray.Capacity - using List now
                Debug.LogError($"Leaderboard size set: {s} is too large!! maximum: {leaderboardArray.Capacity}");
                _leaderboardSize = leaderboardArray.Capacity;
            }
            else if (s < leaderboardArray.Capacity)
            {
                _leaderboardSize = s;
            }
        }

    }
}
