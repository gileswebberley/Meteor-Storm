using System.Collections;
using System.Collections.Generic;
using System;

namespace OodModels
{

    //Templating seemed to make it work, ie <ScoreData>
    [System.Serializable]
    public class ScoreDataList<ScoreData>
    {
        public List<ScoreData> list = new List<ScoreData>(100);
    }

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
            if(y == null)return 1;
            //Console.WriteLine($"Comparing scores - {this.score} and {y.score}");
            //else
            return y.score.CompareTo(this.score);//this.score - y.score;
        }
    }
}

