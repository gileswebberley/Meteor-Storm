using System;

namespace OodModels
{
    public class ScoreManager
    {
        //I was about to make this a singleton but there's some doubt about whether that's appropriate
        //if I did then I wouldn't be able to use several instances in a, let's call it, MultiScoreManager
        //this could then implement a collection of ScoreManagers each with their own score? But I want to
        //reference this from several other objects which is why it seems tempting :/
        protected ScoreData _thisScore = new ScoreData();
        public ScoreData data {
            get {return _thisScore;}
        } 
        //add in a dictionary that holds various scores and then make it a static
        //singleton so everyone can access rather than having to find the object created
        //in another object so _thisScore will be set according to the name of the score 
        //referenced by name of the dictionary entry

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

        public void ResetScore()
        {
            _thisScore.score = 0;
        }
    }
}
