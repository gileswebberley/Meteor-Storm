using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game objects
using TMPro;
//need access to buttons so that we can setup the event on them
using UnityEngine.UI;
//for checking which scene we're in
using UnityEngine.SceneManagement;
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

    [SerializeField] private string[] gameSceneName;

    //A Game USES-A (Aggregation?) spawn, score, difficulty (and leaderboard)
    //but must surely HAVE_A (Composition?) player?
    //reference to Spawn Manager via ISpawnable interface - abstraction (and encapsulation) I think?
    private ISpawnable spawn;
    //reference to the Player
    private PlayerController player;
    //Game Over page with restart button
    private GameObject gameOverUI;
    //private int score = 0;
   
    //This runs before the Start() method, use to instantiate objects of a class
    void Awake()
    {
        //make sure there's only one otherwise it's not much of a singleton
        if (_instance != null)
        {
            //this is not the first instance of MainManager so our singleton already exists
            Destroy(gameObject);
        }
        _instance = this;
        //to make persistant between scenes
        DontDestroyOnLoad(gameObject);
        
        //check the array of scene names that we should be managing as game play scenes
        if (Array.Exists(gameSceneName,element => element == SceneManager.GetActiveScene().name))
        {
            Debug.Log("We have entered a game play scene so game manager is setting up the components");
            //Now these are used to setup the game...
            SetUpGameBounds();
            SetUpDifficulty();
            ScoringSystem.Instance.Scorer.AddToScore(0);
            //gameOverUI.SetActive(false);
            //ScoringSystem.Instance.Scorer.Hide();
            if(ScoringSystem.Instance.Leaderboard == null) ScoringSystem.Instance.SetupLeaderboard(SceneManager.GetActiveScene().name);
            CreatePlayer();
            CreateSpawner();
            StartGame();
        }
    }

    public void CreatePlayer()
    {
        if(player != null) return;
        //Instead of finding the player we'll just load the prefab from Resources folder
        //this way we control when we want it and so don't get all the Null Reference Exceptions
        //explicit because System has Object as well, I'm using that for the Array.Exists()
        GameObject playerGO = UnityEngine.Object.Instantiate(Resources.Load("Player")) as GameObject;
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
        if(spawn != null) return;
        //same for spawner
        GameObject spawnGO = UnityEngine.Object.Instantiate(Resources.Load("Spawn Manager")) as GameObject;
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
        gameOverUI = UnityEngine.Object.Instantiate(Resources.Load("Game Over Page")) as GameObject;
        //pass on the name of the resource to the new GameObject
        gameOverUI.name = "Game Over Page";
        //No script component to grab in here but we will look for buttons
        Button restartButton = gameOverUI.transform.Find("Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(RestartGame);
        Button quitButton = gameOverUI.transform.Find("Quit").GetComponent<Button>();
        quitButton.onClick.AddListener(QuitGame);
        //seems to work very nicely
        if (gameOverUI == null)
        {
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT   - No prefab named 'Game Over Page' in the Assets/Resources folder");
        }
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
        DifficultyManager.mono = this as MonoBehaviour;
    }

    //UI moved to ScoreManagerUI so it looks after itself
    // [Deprecated]
    // public void UpdateScore(int toAdd)
    // {
    //     scorer.AddToScore(toAdd);
    //     //using c# "string interpolation" by using the $ before the string values can be 
    //     //put directly into the string without concatenation by placing them in {}
    //     //scoreText.text = $"{scorer.name} score:{scorer.score}";//+score;
    // }

    public void GameOver()
    {
        Debug.Log("GAME OVER has been called on Game Manager");
        //leave the score visible - no - replaced with ranking message below
        ScoringSystem.Instance.Scorer.Hide();
        gameOver = true;
        //Grabs the asset from Resources folder and attaches event handlers to the button(s)
        CreateGameOver();
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        DifficultyManager.Instance.StopDifficultyStepTimer();
        //adding to the leaderboard
        ScoringSystem.Instance.Leaderboard.AddToLeaderboard(ScoringSystem.Instance.Scorer.data);
        //now find how this score ranked and write a message if it made it onto the leaderboard
        int ranking = ScoringSystem.Instance.Leaderboard.GetLeaderboardRanking(ScoringSystem.Instance.Scorer.data);
        if (ranking != 0)
        {
            gameOverUI.transform.Find("Score Ranking").GetComponent<TextMeshProUGUI>().text = $"{ScoringSystem.Instance.Scorer.name} your score of {ScoringSystem.Instance.Scorer.score} ranks you #{ranking}";
        }
        else
        {
            gameOverUI.transform.Find("Score Ranking").GetComponent<TextMeshProUGUI>().text = $"Sorry {ScoringSystem.Instance.Scorer.name} but you didn't make it onto the leaderboard this time";
        }
    }

    public void StartGame()
    {
        //ScoringSystem.Instance.Leaderboard.LoadLeaderboard(); - I have made this part of the constructor
        gameOver = false;
        ScoringSystem.Instance.Scorer.Show();
        //scoreText.gameObject.SetActive(true);
        //DifficultyManager.Instance.SetDifficulty(1);
        //make an auto difficulty change happen every difficultyChangeTime(seconds)
        //if set to zero the artist does not want to use this functionality
        if(difficultyChangeTime != 0) DifficultyManager.Instance.StartDifficultyStepTimer(difficultyChangeTime);

        //just for testing as something was awry
        // List<ScoreData> lb = ScoringSystem.Instance.Leaderboard.GetLeaderboard();
        // foreach (ScoreData s in lb)
        // {
        //     Debug.Log($"Score {lb.IndexOf(s)}: {s.name} : {s.score}");
        // }
        player.EnablePlayer();
        spawn.RestartSpawn();
    }

    //function attached to the restart button
    public void RestartGame()
    {
        //We don't need to set this in StartGame() as it is initialised with a value of 0
        ScoringSystem.Instance.Scorer.ResetScore();
        //then update the score UI - no need any more as the UI has moved into ScoreManagerUI
        //UpdateScore(scorer.score);
        //trying to fix the bugs that have appeared, getting an error about _mono so redoing the setup
        SetUpDifficulty();
        Destroy(gameOverUI);
        //and (re)start the game...
        StartGame();
    }

    //Quit function that works in both Editor and application attached to Quit button
    public void QuitGame()
    {
        //leaderboard.SaveLeaderboard();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}