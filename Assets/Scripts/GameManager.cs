using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game objects
using TMPro;
//need access to buttons so that we can setup the event on them
using UnityEngine.UI;
using GilesScoreSystem;
using GilesSpawnSystem;
//for DifficultyManager and GameBounds
using GilesManagers;

//Introducing a TagManager to look after all the string literals - like names of prefabs and game objects
public class GameManager : MonoBehaviour
{
    //create static singleton Instance
    protected static GameManager _instance;
    //then the Property to encapsulate it
    public static GameManager Instance
    {
        get { return _instance; }
    }

    private bool gameOver = false;

    //How long to wait before upping the difficulty, default 1 minute
    [SerializeField] private int difficultyChangeTime = 60;

    //this is untested except with a single entry and no scene changes at the moment
    //+ Now in SceneDirector
    //[SerializeField] private string[] gameSceneName;

    //A Game USES-A (Aggregation?) difficulty, boundaries?
    //but must surely HAVE_A (Composition?) player and spawner?
    //reference to Spawn Manager via ISpawnable interface - abstraction (and encapsulation) I think?
    ////"Spawn Manager" must be a prefab in the Resources folder
    private ISpawnable spawn;
    //"Player" must be a prefab in the Resources folder
    private PlayerController player;
    //"Game Over Page" must be a prefab in the Resources folder
    //it must have buttons named "Restart" and "Quit" and a TextMeshProUIGUI named "Score Ranking"
    private GameObject gameOverUI;
    //private int score = 0;

    //This runs before the Start() method, use to instantiate objects of a class
    void Awake()
    {
        //make sure there's only one otherwise it's not much of a singleton
        if (_instance != null)
        {
            //this is not the first instance of GameManager so our singleton already exists
            Destroy(gameObject);
        }
        _instance = this;
        //to make persistant between scenes
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) PauseGame();
    }

    //called from SceneDirector when in a game-play scene
    public void SetUpGame()
    {
        SetUpGameBounds();
        SetUpDifficulty();
        ScoringSystem.Instance.Scorer.AddToScore(0);
        //if there's no leaderboard create/load it according to name set in the ScoringSystem prefab
        if (ScoringSystem.Instance.Leaderboard == null) ScoringSystem.Instance.SetupLeaderboard(ScoringSystem.Instance.leaderboardName);
        CreatePlayer();
        CreateSpawner();
    }

    public void CreatePlayer()
    {
        if (player != null) return;
        //Instead of finding the player we'll just load the prefab from Resources folder
        //this way we control when we want it and so don't get all the Null Reference Exceptions
        //explicit because System has Object as well, I'm using that for the Array.Exists()
        GameObject playerGO = UnityEngine.Object.Instantiate(Resources.Load(TagManager.PLAYER)) as GameObject;
        //set the name so that the MoveForwardRb can still find the speed
        playerGO.name = TagManager.PLAYER;
        //a little attempt at extending TagManager to include component Types
        //System.Type tmpP = System.Type.GetType(TagManager.PLAYER_TYPE);
        player = playerGO.GetComponent<PlayerController>();
        if (player == null)
        {
            Debug.LogError($"GAME MANAGER IS MISSING A VITAL GAMEOBJECT  - No prefab named {TagManager.PLAYER} in the Assets/Resources folder");
        }
    }

    public void CreateSpawner()
    {
        if (spawn != null) return;
        //same for spawner
        GameObject spawnGO = UnityEngine.Object.Instantiate(Resources.Load(TagManager.SPAWN)) as GameObject;
        spawnGO.name = TagManager.SPAWN;
        spawn = spawnGO.GetComponent<ISpawnable>();
        if (spawn == null)
        {
            Debug.LogError($"GAME MANAGER IS MISSING A VITAL GAMEOBJECT  - No prefab named {TagManager.SPAWN} in the Assets/Resources folder");
        }
    }

    public void CreateGameOver()
    {
        if (gameOverUI != null) return;
        //Build the game over page from a prefab in Resources folder, like Player and SpawnManager
        //I'm doing this as I want to use GameManager to create several scenes (just for experimentation) and want to avoid all
        //of the Null Pointer exceptions that you get when you try to use a singleton in Unity it seems?
        gameOverUI = UnityEngine.Object.Instantiate(Resources.Load(TagManager.GAMEOVERUI)) as GameObject;
        //pass on the name of the resource to the new GameObject
        gameOverUI.name = TagManager.GAMEOVERUI;
        //No script component to grab in here but we will look for buttons
        Button restartButton = gameOverUI.transform.Find(TagManager.GAMEOVERRESTARTBUTTON).GetComponent<Button>();
        restartButton.onClick.AddListener(RestartGame);
        Button quitButton = gameOverUI.transform.Find(TagManager.GAMEOVERQUITBUTTON).GetComponent<Button>();
        //must come back to work out how to add a method with a parameter - fix for now
        quitButton.onClick.AddListener(SceneDirector.GoToStart);
        //seems to work very nicely
        if (gameOverUI == null)
        {
            Debug.LogError($"GAME MANAGER IS MISSING A VITAL GAMEOBJECT   - No prefab named {TagManager.GAMEOVERUI} in the Assets/Resources folder");
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
        DifficultyManager.Instance.mono = this as MonoBehaviour;
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
        //set the game over flag
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
            gameOverUI.transform.Find(TagManager.GAMEOVERSCORETEXT).GetComponent<TextMeshProUGUI>().text = $"{ScoringSystem.Instance.Scorer.name} your score of {ScoringSystem.Instance.Scorer.score} ranks you #{ranking}";
        }
        else
        {
            gameOverUI.transform.Find(TagManager.GAMEOVERSCORETEXT).GetComponent<TextMeshProUGUI>().text = $"Sorry {ScoringSystem.Instance.Scorer.name} but you didn't make it onto the leaderboard this time";
        }
    }

    public void StartGame()
    {
        gameOver = false;
        ScoringSystem.Instance.Scorer.Show();
        //make an auto difficulty change happen every difficultyChangeTime(seconds)
        //if set to zero the artist does not want to use this functionality
        if (difficultyChangeTime != 0) DifficultyManager.Instance.StartDifficultyStepTimer(difficultyChangeTime);
        player.EnablePlayer();
        spawn.RestartSpawn();
    }

    //very basic pause function
    public void PauseGame()
    {
        //simply switch from current state to other state
        Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
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
}