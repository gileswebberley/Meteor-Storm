using System.Collections.Generic;
using UnityEngine;

namespace OodModels
{
    //This was originally ScoreManager! realised that this is something to use WITH scores not FOR them
    public abstract class Leaderboard
    {
        //make the leaderboard itself large and then use leaderboardSize for what we work with? 
        //Fixed - Sort() is not working as expected
        [SerializeField] protected ScoreDataList<ScoreData> leaderboardArray = new ScoreDataList<ScoreData>();

        //default constructor
        public Leaderboard()
        {
            leaderboardArray.list.Capacity = _leaderboardCapacity;
        }

        //just so we keep control of the size of memory used
        private int _leaderboardCapacity = 100;
        public int leaderboardCapacity {
            get {return _leaderboardCapacity;}
            set {
                _leaderboardCapacity = value;
                leaderboardArray.list.Capacity = _leaderboardCapacity;
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
        public abstract void SaveLeaderboard();

        //Provided in here as it is only dealing with the underlying data
        protected void AddToLeaderboardRaw(ScoreData newScore)
        {
            if (leaderboardArray.list != null)
            {
                leaderboardArray.list.Add(newScore);
                //then sort based on the ScoreData Comparable
                leaderboardArray.list.Sort();

                Debug.Log($"New score (rank): {newScore.name} : {newScore.score} ({leaderboardArray.list.IndexOf(newScore)+1}) added to the {_leaderboardName} leaderboard");
                //if we're about to resize the list clip off the last entry to enforce capacity
                if (leaderboardArray.list.Count >= leaderboardArray.list.Capacity - 1)
                {
                    //then get rid of the lowest score
                    leaderboardArray.list.RemoveAt(leaderboardArray.list.Count);
                }
            }else{
                Debug.LogError("Leaderboard is NULL in AddToLeaderboard()");
            }
        }

        public virtual int GetLeaderboardRanking(ScoreData theScore)
        {
            int ranking;
            //IndexOf returns -1 if the entry is not found, so it will return zero if not on the leaderboard
            ranking = leaderboardArray.list.IndexOf(theScore)+1;
            if(ranking > _leaderboardSize) ranking = 0;
            if(ranking > 0){
                Debug.Log($"New score (rank): {theScore.name} : {theScore.score} ({ranking}) added to the {_leaderboardName} leaderboard");
            }else{
                Debug.Log($"Unfortunately {theScore.name} you didn't make it onto the {_leaderboardName} leaderboard this time");
            }
            return ranking;
        }

        public virtual List<ScoreData> GetLeaderboard(){
            List<ScoreData> temp = new List<ScoreData>(_leaderboardSize);
            for(int i = 0; i < _leaderboardSize; i++){
                //Catch Out Of Bounds exceptions in case the list is shorter than _leaderboardSize
                try{
                    temp.Add(leaderboardArray.list[i]);
                }catch{
                    temp.Add(new ScoreData());
                }
            }  
            //List<ScoreData> temp = leaderboardArray.list.GetRange(0,_leaderboardSize);
            return temp;
        }

        private void ChangeLeaderboardSize(int s)
        {
            //keep the array tidy
            if (s > leaderboardArray.list.Capacity)
            {
                //it's bigger than the leaderboardArray.list.Capacity - using List now
                Debug.LogError($"Leaderboard size set: {s} is too large!! maximum: {leaderboardArray.list.Capacity}");
                _leaderboardSize = leaderboardArray.list.Capacity;
            }
            else if (s < leaderboardArray.list.Capacity)
            {
                _leaderboardSize = s;
            }
        }

    }
}
