using System;
using UnityEngine;

namespace OodModels
{
    //as I extended this to be ScoreManagerUI I had trouble with using the new keyword so try making it an SO
    public class ScoreManager : ScriptableObject
    {
        //I was about to make this a singleton but there's some doubt about whether that's appropriate
        //if I did then I wouldn't be able to use several instances in a, let's call it, MultiScoreManager
        //this could then implement a collection of ScoreManagers each with their own score? But I want to
        //reference this from several other objects which is why it seems tempting :/ No, I think that what
        //ScoreData is for so just inherit from this and simply set _thisScore to whichever one we're working with.

        //this is static so that it is persistent across instances in scenes - had a bit of trouble during 
        //the creation of the Sign Up process and the scene changes therein
        protected static ScoreData _thisScore = new ScoreData();
        public ScoreData data {
            get {
                if(_thisScore == null){                    
                    Debug.Log($"ScoreManager is creating a new ScoreData whilst being fetched");
                     _thisScore = new ScoreData();
                }
                return _thisScore;
                }
        } 
        //add in a dictionary that holds various scores and then make it a static
        //singleton so everyone can access rather than having to find the object created
        //in another object so _thisScore will be set according to the name of the score 
        //referenced by name of the dictionary entry

        //score will always remain above or at zero
        //protected int _score = 0;
        public int score {
            //using data property to access to check there's an instance of ScoreData
            get{return data.score;}
        }

        //new keyword used as it is hiding Object.name
        public new string name {
            get {
                Debug.Log($"ScoreManager _thisScore.name is being fetched as: {_thisScore.name}");
                return _thisScore.name;}
            //late addition but there doesn't seem any good reason to protect this from setting
            set {
                Debug.Log($"ScoreManager _thisScore.name is being set to: {value}");
                _thisScore.name = value;}
        }

        //only use for positive numbers, a mistaken minus will be passed to remove function
        public virtual void AddToScore(int toAdd)
        {
            if(toAdd < 0){
                RemoveFromScore(toAdd*-1);
            }else{
                _thisScore.score += toAdd;
            }
        }
        //reverse functionality of AddToScore
        public virtual void RemoveFromScore(int toRemove)
        {
            if(toRemove < 0){
                AddToScore(toRemove*-1);
            }else if(_thisScore.score <= toRemove){
                _thisScore.score = 0;
            }
        }

        public virtual void ResetScore()
        {
            _thisScore.score = 0;
        }
    }
}
