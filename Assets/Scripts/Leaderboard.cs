using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OodModels
{
    //This was originally ScoreManager! realised that this is something to use WITH scores not FOR them
    public class Leaderboard
    {
        // protected int _leaderboardCapacity = 100;
        // public int leaderboardCapacity {
        //     get {return _leaderboardCapacity;}
        //     set {leaderboardArray.Capacity = value;}//this could be dangerous?
        // }
        //make the leaderboard itself large and then use leaderboardSize for what we work with? 
        protected List<ScoreData> leaderboardArray = new List<ScoreData>(100);

        protected int _leaderboardSize = 10;
        public int leaderboardSize
        {
            get { return _leaderboardSize; }
            set { ChangeLeaderboardSize(value); }
        }

        //want to abstract out the way it's stored -----
        protected string _fileName = "Leaderboard-Default";
        public string leaderboardName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        protected void ChangeLeaderboardSize(int s)
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

        public virtual void AddToLeaderboard(string name, int score)
        {
            ScoreData newScore = new ScoreData(name, score);
            AddToLeaderboard(newScore);
        }

        public virtual void AddToLeaderboard(ScoreData newScore)
        {
            LoadLeaderboard(_fileName);
            AddToLeaderboardRaw(newScore);
            SaveLeaderboard(_fileName);
        }

        //To abstract out the loading behaviour (where and how it's stored)
        protected void AddToLeaderboardRaw(ScoreData newScore)
        {
            if (leaderboardArray != null)
            {
                //--------- extract out
                leaderboardArray.Add(newScore);
                //then sort based on the ScoreData Comparable I hope
                leaderboardArray.Sort();
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

        public void LoadLeaderboard(string name)
        {
            //set the internal file name for AddToLeaderboard() auto load/save
            _fileName = name;
            if (File.Exists(Application.persistentDataPath + "/" + name + ".json"))
            {
                string jsonStr = File.ReadAllText(Application.persistentDataPath + "/" + name + ".json");
                leaderboardArray = JsonUtility.FromJson<List<ScoreData>>(jsonStr);
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
            }
        }

        public void SaveLeaderboard(string name)
        {
            _fileName = name;
            string jsonStr = JsonUtility.ToJson(leaderboardArray);
            File.WriteAllText(Application.persistentDataPath + "/" + name + ".json", jsonStr);
        }
    }
}
