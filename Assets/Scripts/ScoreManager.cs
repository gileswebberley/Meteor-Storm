using System;

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
}
