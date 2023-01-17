using System.Collections.Generic;

namespace OodModels
{
    //Templating seemed to make it work, ie <ScoreData>
    [System.Serializable]
    public class ScoreDataList<ScoreData>
    {
        public List<ScoreData> list = new List<ScoreData>(100);
    }
}

