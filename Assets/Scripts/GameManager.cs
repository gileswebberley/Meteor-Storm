using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game object
using TMPro;
//need access to buttons so that we can setup the event on them
using UnityEngine.UI;
//to set GameBounds for bounds checking across the application
using OodModels;
//for ISpawnable
using IModels;

public class GameManager : MonoBehaviour
{
    //create singleton Instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
        protected set { _instance = value; }
    }
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
        //gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(false);
        StartGame();
    }
    //This runs before the Start() method, use to instantiate objects of a class
    void Awake()
    {
        //make sure there's only one otherwise it's not much of a singleton
        if (Instance != null)
        {
            //this is not the first instance of MainManager so our singleton already exists
            Destroy(gameObject);
        }
        Instance = this;
        //to make persistant between scenes
        DontDestroyOnLoad(gameObject);
        
        SetUpGameBounds();
        SetUpDifficulty();
        SetUpScoring("Giles","MeteorStorm");
        CreatePlayer();
        CreateSpawner();
    }

    public void CreatePlayer()
    {
        //Instead of finding the player we'll just load the prefab from Resources folder
        //this way we control when we want it and so don't get all the Null Reference Exceptions
        GameObject playerGO = Object.Instantiate(Resources.Load("Player")) as GameObject;
        //set the name so that the MoveForwardRb can still find the speed
        playerGO.name = "Player";
        player = playerGO.GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT - No prefab named 'Player' in the Assets/Resources folder");
        }
    }

    public void CreateSpawner()
    {
        //spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        
        //same for spawner
        GameObject spawnGO = Object.Instantiate(Resources.Load("Spawn Manager")) as GameObject;
        spawnGO.name = "Spawn Manager";
        spawn = spawnGO.GetComponent<ISpawnable>();
        if (spawn == null)
        {
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT  - No prefab named 'Spawn Manager' in the Assets/Resources folder");
        }
    }

    public void CreateGameOver()
    {
        //Build the game over page from a prefab in Resources folder, like Player and SpawnManager
        //I'm doing this as I want to use GameManager to create several scenes (just for experimentation) and want to avoid all
        //of the Null Pointer exceptions that you get when you try to use a singleton in Unity it seems?
        gameOverUI = Object.Instantiate(Resources.Load("Game Over Page")) as GameObject;
        //pass on the name of the resource to the new GameObject
        gameOverUI.name = "Game Over Page";
        //No script component to grab in here but we will look for buttons
        //gameOverUI = transform.Find("Game Over Page").gameObject;
        Button restartButton = gameOverUI.transform.Find("Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(RestartGame);
        //seems to work very nicely
        if (gameOverUI == null)
        {
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT   - No prefab named 'Game Over Page' in the Assets/Resources folder");
        }
    }

    public void SetUpScoring(string scoreName, string leaderboardName)
    {
        //Try using score manager....
        scorer = new ScoreManager();
        scorer.name = scoreName;
        //and now a leaderboard...
        leaderboard = new FileLeaderboard(leaderboardName);
    }

    public void SetUpGameBounds()
    {
        //Set up our centralised playable bounds to reduce the need for player and spawner to communicate
        //minBounds.z is the "closest" z-value - used for bounds checking in MoveForward
        GameBounds.minBounds = new Vector3(-50, -50, 10);
        //maxBounds.z is the "farthest away" z-value
        GameBounds.maxBounds = new Vector3(50, 50, -800);
    }

    public void SetUpDifficulty()
    {
        //Set up the static properties for difficulty
        DifficultyManager.maxDifficulty = 5;
        DifficultyManager.difficulty = 1;
        //pass this MonoBehaviour object to run co-routines (for the timer)
        DifficultyManager.mono = this;
    }

    public void UpdateScore(int toAdd)
    {
        scorer.AddToScore(toAdd);
        //using c# "string interpolation" by using the $ before the string values can be 
        //put directly into the string without concatenation by placing them in {}
        scoreText.text = $"{scorer.name} score:{scorer.score}";//+score;
    }

    public void GameOver()
    {
        //leave the score visible
        gameOver = true;
        //Grabs the asset from Resources folder and attaches event handlers to the button(s)
        CreateGameOver();
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        DifficultyManager.Instance.StopDifficultyStepTimer();
        //adding to the leaderboard
        leaderboard.AddToLeaderboard(scorer.data);
    }

    public void StartGame()
    {
        leaderboard.LoadLeaderboard();
        gameOver = false;
        //gameOverUI.SetActive(false);
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
        player.EnablePlayer();
        spawn.RestartSpawn();
    }

    public void RestartGame()
    {//function attached to the restart button
        //We don't need to set this in StartGame() as it is initialised with a value of 0
        scorer.ResetScore();
        //then update the score UI
        UpdateScore(scorer.score);
        
        Destroy(gameOverUI);
        //and (re)start the game...
        StartGame();
    }
}