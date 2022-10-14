using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game object
using TMPro;

public class GameManager : MonoBehaviour {
    private bool gameOver = false;
    public float playerPower = 10f;
    public float playerStrength = 100f;

    private int difficulty = 1;
    private int maxDifficulty = 3;
    //How long to wait before upping the difficulty, default 1 minute
    [SerializeField] private int difficultyChangeTime = 60;
    //This is why we've added the TMPro library
    [SerializeField] private TextMeshProUGUI scoreText;

     //reference to Spawn Manager
     private SpawnManager spawn;
     //reference to the Player
     private PlayerController player;
     //Game Over page with restart button
     private GameObject gameOverUI;
     private int score = 0;

    void Start(){
        UpdateScore(0);
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(false);
        //finally got this woking after discovering about instantiating within the Awake()
        StartGame();
    }
    //This runs before the Start() method, use to instantiate objects of the class
    void Awake(){
        gameOverUI = transform.Find("Game Over Page").gameObject;
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if(spawn == null || player == null){
            Debug.LogError("PLAYER OR SPAWN IS NULL TO GAME MANAGER");
        }
    }

    void Update(){

    }

    IEnumerator DifficultyChangeTimer(){
        //don't run if difficulty is already topped out
        if(difficulty >= maxDifficulty)yield break;
        //else wait for the time
        yield return new WaitForSeconds(difficultyChangeTime);
        //then add 1 to difficulty
        SetDifficulty(difficulty+1);
        //then resursively call this iterator
        StartCoroutine("DifficultyChangeTimer");
    }

    public int GetDifficulty(){
        return difficulty;
    }
    //Difficulty bounded to [1..3]
    public void SetDifficulty(int aDif){
        difficulty = aDif;
        if(difficulty < 1){
            difficulty = 1;
        }
        else if(difficulty > maxDifficulty){
            difficulty = maxDifficulty;
            //stop wasting proccesing if difficulty is already at it's highest
            //StopCoroutine("DifficultyChangeTimer");
        }
        Debug.Log("DIfficulty is now: "+difficulty);
    }

    public void UpdateScore(int toAdd){
        score += toAdd;
        scoreText.text = "score: "+score;
    }

    public void GameOver(){
        //leave the score visible
        gameOver = true;
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        SetDifficulty(1);
        StopCoroutine("DifficultyChangeTimer");
    }

    public void StartGame(){
        gameOver = false;
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(true);
        //to chek that starting difficulty is within range
        SetDifficulty(difficulty);
        StartCoroutine("DifficultyChangeTimer");
        //I don't know why this makes everything stop working, 
        //only change is calling from here rather than from the 
        //Start() in each of the class definitions - use Awake()
        player.EnablePlayer(playerStrength,playerPower);
        spawn.RestartSpawn();
    }

    public void RestartGame(){//function attached to the restart button
        //We don't need to set this in StartGame() as it is initialised with a value of 1
        UpdateScore(-score);
        StartGame();
    }
}