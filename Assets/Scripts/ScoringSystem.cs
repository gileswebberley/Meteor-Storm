using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OodModels;
using TMPro;

/*
So, I had all of this in the game manager because I started this project very early in
my Unity and C# learning process; I had one scene with a simple variable for the score.
As I have continued to try to refactor this into a decent shape and learn as much as I 
can through trial and error. I have made a leaderboard and scoring classes to understand
various concepts and now I have started to make a scene structure and so I want this seperate
for entering a player's name before playing the game - that's why it doesn't feel correct
to still have it tied up with the game manager (perhaps that doesn't need to be a singleton
in the end? - no, I might want it across levels?)
*/

//Brings together the score manager and the leaderboard
public class ScoringSystem : MonoBehaviour
{
    //hold the singleton object reference all the work is done in Awake()
    private static ScoringSystem _instance;
    public static ScoringSystem Instance 
    {
        get {return _instance;}
        protected set {_instance = value;}
    }

    //This is why we've added the TMPro library
    //This is the name of the text area used for the score - default set
    [SerializeField, Tooltip("Create a TMPro text area and place in your scene (ensure it is Enabled) - Enter the name of the object here")]
    private string scoreTMPName = "Score Text";
    //score replaced by a score manager which the game manager has access to
    private static ScoreManagerUI scorer;
    public ScoreManagerUI Scorer
    {
        get { return scorer; }
        //protected set {scorer = value;}
    }

    //time to test out a leaderboard
    private static FileLeaderboard leaderboard;
    public FileLeaderboard Leaderboard
    {
        get{return leaderboard;}
        //protected set {leaderboard = value;}
    }

    //as far as I understand this will run when a new scene is loaded whereas Start will not?
    void Awake()
    {
        //make sure there's only one otherwise it's not much of a singleton
        if (Instance != null)
        {
            //this is not the first instance of ScoreManager so our singleton already exists
            Destroy(gameObject);
        }
        Instance = this;
        //to make persistant between scenes
        DontDestroyOnLoad(gameObject);

        // //Try using score manager....experimenting with ScriptableObjects...
        // //scorer = new ScoreManagerUI(); - below is how to do this with an SO
        if(scorer == null) scorer = ScriptableObject.CreateInstance("ScoreManagerUI") as ScoreManagerUI;
        // //now we search the scene that's just loaded to see if we can find an appropriately named text area
        scorer.CreateScoreTextArea(scoreTMPName);
        SetupLeaderboard("MeteorStorm");
    }

    public void PrintLeaderboard(TextMeshProUGUI textArea)
    {
        textArea.alignment = TextAlignmentOptions.Center;
        textArea.text = "LEADERBOARD\n\nRank : Name : Score\n";
        List<ScoreData> lb = leaderboard.GetLeaderboard();
        foreach (ScoreData s in lb)
        {
            textArea.text += $"{lb.IndexOf(s)+1} : {s.name} : {s.score}\n";
            //Debug.Log($"Score {lb.IndexOf(s)}: {s.name} : {s.score}");
        }
    }
    public void SetupLeaderboard(string leaderboardName)
    {
        if(leaderboard != null) return;     
        //if the leaderboardName is already loaded it 
        leaderboard = new FileLeaderboard(leaderboardName);
    }

    public void SetUpScoringName(string scoreName)
    {   
        Debug.Log($"Setting score name to {scoreName}");    
        scorer.name = scoreName;
    }
}
