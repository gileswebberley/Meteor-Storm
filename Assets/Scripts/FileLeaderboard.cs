using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GilesScoreSystem
{
    public class FileLeaderboard : Leaderboard
    {
        public FileLeaderboard(string name){
            //if it's already loaded the same one then don't bother - is this correct?
            if(name != _leaderboardName) LoadLeaderboard(name);            
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
            AddToLeaderboardRaw(newScore);//should this be base.AddToLeaderboard(ScoreData)? no, only when overridden.
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
                //this logic is now in LoadLeaderboard when it doesn't exist
                //SaveLeaderboard();
                Debug.LogError($"LOAD FAILED: leaderboard named {name} did not exist and so a default leaderboard has been constructed");
            }
        }

        //will load leaderboard set via leaderboardName property directly from the user object
        public override bool LoadLeaderboard(){
            if (File.Exists(Application.persistentDataPath + "/" + _leaderboardName + ".json"))
            {
                string jsonStr = File.ReadAllText(Application.persistentDataPath + "/" + _leaderboardName + ".json");
                //not working so tested with saving a single ScoreData object which worked....
                leaderboardArray = JsonUtility.FromJson<ScoreDataList<ScoreData>>(jsonStr);
                //Debug.Log($"LoadLeaderboard {_leaderboardName} : {leaderboardArray[0]}");
                return true;
            }
            else
            {
                //set up a default leaderboardArray of leaderboardSize length
                //check this hasn't already made excess entries
                //if(leaderboardArray.Count < leaderboardSize){
                int i = 0;
                do
                {
                    leaderboardArray.list.Add(new ScoreData());//fill with ScoreData default constructor
                    //Debug.Log($"{i+1} Scores added to leaderboard in LoadLeaderboard()");
                    ++i;
                } while (i < leaderboardSize);

                //save the default leaderboard now we've created a blank one
                SaveLeaderboard();
                return false;
                }
                
            //}
        }

        //Like the Save As... functionality
        public override void SaveLeaderboard(string name)
        {
            string jsonStr = JsonUtility.ToJson(leaderboardArray);
            Debug.Log($"SaveLeaderboard({name}) : {jsonStr}");
            File.WriteAllText(Application.persistentDataPath + "/" + name + ".json", jsonStr);
        }

        //and the save the last loaded leaderboard
        public override void SaveLeaderboard()
        {
            //FIXED - (NOT WORKING - producing "{}" and that's it) - Implemented ScoreDataList<ScoreData> to fix
            // string jsonStr = JsonUtility.ToJson(leaderboardArray);
            // File.WriteAllText(Application.persistentDataPath + "/" + _leaderboardName + ".json", jsonStr);
            SaveLeaderboard(_leaderboardName);
            //return true;
        }

        public override List<ScoreData> GetLeaderboard()
        {
            LoadLeaderboard();
            return base.GetLeaderboard();
        }
    }
}
