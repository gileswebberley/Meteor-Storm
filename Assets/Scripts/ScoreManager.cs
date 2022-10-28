using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace OodModels
{
    public class ScoreManager
    {
        protected ScoreData _thisScore = new ScoreData();
        public ScoreData data {
            get {return _thisScore;}
        } 
        //score will always remain above or at zero
        //protected int _score = 0;
        public int score {
            get{return _thisScore.score;}
        }

        public string name {
            get {return _thisScore.name;}
        }

        public void AddToScore(int toAdd)
        {
            if(toAdd < 0){
                RemoveFromScore(toAdd*-1);
            }else{
                _thisScore.score += toAdd;
            }
        }

        public void RemoveFromScore(int toRemove)
        {
            if(toRemove < 0){
                AddToScore(toRemove*-1);
            }else if(_thisScore.score <= toRemove){
                _thisScore.score = 0;
            }
        }
    }

    //This was originally ScoreManager! realised that this is something to use WITH scores not FOR them
    public class Leaderboard
    {
        //make the leaderboard itself large and then use leaderboardSize for what we work with? 
        protected List<ScoreData> leaderboardArray = new List<ScoreData>(100);//will that fill with defaults?

        protected int _leaderboardSize = 10;
        public int leaderboardSize
        {
            get { return _leaderboardSize; }
            set { ChangeLeaderboardSize(value); }
        }

        protected string _fileName = "Leaderboard-Default.json";
        public string fileName
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

        public void AddToLeaderboard(string name, int score)
        {
            ScoreData newScore = new ScoreData(name, score);
            AddToLeaderboard(newScore);
        }

        public void AddToLeaderboard(ScoreData newScore)
        {
            LoadLeaderboard(_fileName);
            leaderboardArray.Add(newScore);
            //then sort based on the ScoreData Comparable I hope
            leaderboardArray.Sort();
            //if we're about to resize the list clip off the last entry to enforce capacity
            if (leaderboardArray.Count >= leaderboardArray.Capacity - 1)
            {
                //then get rid of the lowest score
                leaderboardArray.RemoveAt(leaderboardArray.Count);
            }
            SaveLeaderboard(_fileName);
        }

        public void LoadLeaderboard(string filename)
        {
            //set the internal file name for AddToLeaderboard() auto load/save
            _fileName = filename;
            if (File.Exists(Application.persistentDataPath + "/" + filename))
            {
                string jsonStr = File.ReadAllText(Application.persistentDataPath + "/" + filename);
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

        public void SaveLeaderboard(string filename)
        {
            _fileName = filename;
            string jsonStr = JsonUtility.ToJson(leaderboardArray);
            File.WriteAllText(Application.persistentDataPath + "/" + filename, jsonStr);
        }
    }
}
