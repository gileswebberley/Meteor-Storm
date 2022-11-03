using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OodModels
{
    //This was originally ScoreManager! realised that this is something to use WITH scores not FOR them
    public abstract class Leaderboard
    {
        //make the leaderboard itself large and then use leaderboardSize for what we work with? 
        protected List<ScoreData> leaderboardArray = new List<ScoreData>(100);

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
                temp.Add(leaderboardArray[i]);
            }
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

    public class FileLeaderboard : Leaderboard
    {
        public FileLeaderboard(string name){
            _leaderboardName = name;
        }
        public override void AddToLeaderboard(string name, int score)
        {
            ScoreData newScore = new ScoreData(name, score);
            AddToLeaderboard(newScore);
        }

        //make this abstract
        public override void AddToLeaderboard(ScoreData newScore)
        {
            LoadLeaderboard(_leaderboardName);
            //dealing with the underlying List<ScoreData> so use inherited method
            AddToLeaderboardRaw(newScore);//should this be base.AddToLeaderboard(ScoreData)?
            SaveLeaderboard(_leaderboardName);
        }

        public override void LoadLeaderboard(string name)
        {
            //I'm not sure about whether this should happen in here?? might you want to load another one into here?
            //I've decided that if you load one by name you would expect it to save as that one unless specified
            //so I won't set in the save version (like a Save and Save As... options with a document)
            //set the internal file name for AddToLeaderboard() auto load/save
            _leaderboardName = name;
            if (!LoadLeaderboard())
            {
                //then save it, as if it's been requested and doesn't exist it probably should now?
                SaveLeaderboard();
            }
        }

        //will load leaderboard set via leaderboardName property directly from the user object
        public override bool LoadLeaderboard(){
            if (File.Exists(Application.persistentDataPath + "/" + _leaderboardName + ".json"))
            {
                string jsonStr = File.ReadAllText(Application.persistentDataPath + "/" + _leaderboardName + ".json");
                leaderboardArray = JsonUtility.FromJson<List<ScoreData>>(jsonStr);
                return true;
            }
            else
            {
                //set up a default leaderboardArray of leaderboardSize length
                int i = 0;
                do
                {
                    leaderboardArray.Add(new ScoreData());//fill with ScoreData default constructor
                    ++i;
                } while (i < leaderboardSize);
                return false;
            }
        }

        //Like the Save As... functionality
        public override void SaveLeaderboard(string name)
        {
            //_leaderboardName = name;
            string jsonStr = JsonUtility.ToJson(leaderboardArray);
            File.WriteAllText(Application.persistentDataPath + "/" + name + ".json", jsonStr);
        }

        //and the save the last loaded leaderboard
        public override bool SaveLeaderboard()
        {
            string jsonStr = JsonUtility.ToJson(leaderboardArray);
            File.WriteAllText(Application.persistentDataPath + "/" + _leaderboardName + ".json", jsonStr);
            return true;
        }
    }
}
