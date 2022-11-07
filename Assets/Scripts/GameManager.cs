using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game object
using TMPro;
//to set GameBounds for bounds checking across the application
using OodModels;
//for ISpawnable
using IModels;

public class GameManager : MonoBehaviour
{
    // //create singleton Instance
    // private static GameManager _instance;
    // public static GameManager Instance
    // {
    //     get { return _instance; }
    //     protected set { _instance = value; }
    // }
    private bool gameOver = false;

    //How long to wait before upping the difficulty, default 1 minute
    [SerializeField] private int difficultyChangeTime = 60;
    //This is why we've added the TMPro library
    [SerializeField] private TextMeshProUGUI scoreText;

    //A Game USES-A (Aggregation?) spawn, score, difficulty (and leaderboard)
    //but must surely HAVE_A (Composition?) player?
    //reference to Spawn Manager via ISpawnable interface - abstraction (and encapsulation) I think?
    private ISpawnable spawn;
    //reference to the Player
    private PlayerController player;
    //Game Over page with restart button
    private GameObject gameOverUI;
    //private int score = 0;
    //score replaced by a score manager which the game manager has access to
    private ScoreManager scorer;

    //time to test out a leaderboard
    private FileLeaderboard leaderboard;


    void Start()
    {
        UpdateScore(0);
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(false);
        StartGame();
    }
    //This runs before the Start() method, use to instantiate objects of a class
    void Awake()
    {
        // //make sure there's only one otherwise it's not much of a singleton
        // if (Instance != null)
        // {
        //     //this is not the first instance of MainManager so our singleton already exists
        //     Destroy(gameObject);
        // }
        // Instance = this;
        // //to make persistant between scenes
        // DontDestroyOnLoad(gameObject);
        
        //Set up our centralised playable bounds to reduce the need for player and spawner to communicate
        //minBounds.z is the "closest" z-value - used for bounds checking in MoveForward
        GameBounds.minBounds = new Vector3(-50, -50, 10);
        //maxBounds.z is the "farthest away" z-value
        GameBounds.maxBounds = new Vector3(50, 50, -800);

        //Set up the static properties for difficulty
        DifficultyManager.maxDifficulty = 5;
        DifficultyManager.difficulty = 1;
        //Try to set this calling MonoBehaviour object to run co-routines
        DifficultyManager.mono = this;

        //Try using score manager....
        scorer = new ScoreManager();
        //and now a leaderboard...
        leaderboard = new FileLeaderboard("MeteorStorm");

        //get references to necessary components
        //in oop terms is this a form of Aggregation? 
        gameOverUI = transform.Find("Game Over Page").gameObject;
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if (spawn == null || player == null || gameOverUI == null)
        {
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT REFERENCE");
        }
    }

    public void UpdateScore(int toAdd)
    {
        scorer.AddToScore(toAdd);
        //score += toAdd;
        //using c# "string interpolation" by using the $ before the string values can be 
        //put directly into the string without concatenation by placing them in {}
        scoreText.text = $"{scorer.name} score:{scorer.score}";//+score;
    }

    public void GameOver()
    {
        //leave the score visible
        gameOver = true;
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        DifficultyManager.Instance.StopDifficultyStepTimer();
        //adding to the leaderboard
        leaderboard.AddToLeaderboard(scorer.data);
        //no need for this as it is saved as part of the adding process
        //leaderboard.SaveLeaderboard();
        //just for testing as something is awry
        // List<ScoreData> lb = leaderboard.GetLeaderboard();
        // foreach(ScoreData s in lb)
        // {
        //     Debug.Log($"Score {lb.IndexOf(s)}: {s.name} : {s.score}");
        // }
    }

    public void StartGame()
    {
        leaderboard.LoadLeaderboard();
        gameOver = false;
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(true);
        DifficultyManager.Instance.SetDifficulty(1);
        //make an auto difficulty change happen every difficultyChangeTime(seconds)
        DifficultyManager.Instance.StartDifficultyStepTimer(difficultyChangeTime);
        
        //just for testing as something is awry
        List<ScoreData> lb = leaderboard.GetLeaderboard();
        foreach(ScoreData s in lb)
        {
            Debug.Log($"Score {lb.IndexOf(s)}: {s.name} : {s.score}");
        }
        // ++ use Awake()
        //player.EnablePlayer(playerStrength, playerPower);
        player.EnablePlayer();
        spawn.RestartSpawn();
    }

    public void RestartGame()
    {//function attached to the restart button
        //We don't need to set this in StartGame() as it is initialised with a value of 1
        UpdateScore(-scorer.score);
        StartGame();
    }
}