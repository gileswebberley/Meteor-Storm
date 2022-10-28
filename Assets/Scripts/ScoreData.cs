using System.Collections;
using System;

namespace OodModels
{

    //I believe the IComparable will make Sort<ScoreData>() function based on score
    [System.Serializable]
    public class ScoreData : IComparable<ScoreData>
    {
        //default constructor
        public ScoreData()
        {
            name = "Guest";
            score = 0;
        }

        //overloaded constructor for an initialised ScoreData
        public ScoreData(string n, int s)
        {
            name = n;
            score = s;
        }
        public string name;
        public int score;

        //trying to implement sorting based on score but not sure how to
        public int CompareTo(ScoreData y)
        {
            return this.score - y.score;
        }
    }
}

