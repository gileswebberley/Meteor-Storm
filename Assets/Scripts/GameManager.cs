using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game object
using TMPro;
//to set GameBounds for bounds checking across the application
using OodModels;
//for ISpawnable
using IModels;

public class GameManager : MonoBehaviour {
    private bool gameOver = false;
    public float playerPower = 10f;
    public float playerStrength = 1000f;

    // private int difficulty = 1;
    // private int maxDifficulty = 3;
    //How long to wait before upping the difficulty, default 1 minute
    [SerializeField] private int difficultyChangeTime = 60;
    //This is why we've added the TMPro library
    [SerializeField] private TextMeshProUGUI scoreText;

     //reference to Spawn Manager via ISpawnable interface - abstraction and encapsulation I think?
     private ISpawnable spawn;
     //reference to the Player
     private PlayerController player;
     //Game Over page with restart button
     private GameObject gameOverUI;
     private int score = 0;

    void Start(){
        UpdateScore(0);
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(false);
        StartGame();
    }
    //This runs before the Start() method, use to instantiate objects of a class
    void Awake(){
        //Set up our centralised playable bounds to reduce the need for player and spawner to communicate
        //minBounds.z is the "closest" z-value - used for bounds checking in MoveForward
        GameBounds.minBounds = new Vector3(-50,-50,10);
        //maxBounds.z is the "farthest away" z-value
        GameBounds.maxBounds = new Vector3(50,50,-800);
        
        //Can't find Difficulty Manager??
        DifficultyManager.maxDifficulty = 5;
        DifficultyManager.difficulty = 1;

        //get references to necessary components
        //in oop terms is this a form of Aggregation? 
        gameOverUI = transform.Find("Game Over Page").gameObject;
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if(spawn == null || player == null || gameOverUI == null){
            Debug.LogError("GAME MANAGER IS MISSING A VITAL GAMEOBJECT REFERENCE");
        }
    }

    public void UpdateScore(int toAdd){
        score += toAdd;
        //using c# "string interpolation" by using the $ before the string values can be 
        //put directly into the string without concatenation by placing them in {}
        scoreText.text = $"score:{score}";//+score;
    }

    public void GameOver(){
        //leave the score visible
        gameOver = true;
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        DifficultyManager.difficulty = 1;// SetDifficulty(1);
        DifficultyManager.Instance.StopDifficultyStepTimer();
        //StopCoroutine("DifficultyChangeTimer");
    }

    public void StartGame(){
        gameOver = false;
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(true);
        //make an auto difficulty change happen every difficultyChangeTime(seconds)
        DifficultyManager.Instance.StartDifficultyStepTimer(difficultyChangeTime);
        // ++ use Awake()
        player.EnablePlayer(playerStrength,playerPower);
        spawn.RestartSpawn();
    }

    public void RestartGame(){//function attached to the restart button
        //We don't need to set this in StartGame() as it is initialised with a value of 1
        UpdateScore(-score);
        StartGame();
    }
}