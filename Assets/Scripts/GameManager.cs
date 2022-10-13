using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//add TMPro include to access our text mesh pro game object
using TMPro;

public class GameManager : MonoBehaviour {
    public bool gameOver = false;
    public float playerPower = 10f;
    public float playerStrength = 1000f;

    private int difficulty = 1;
    //How long to wait before upping the difficulty, default 1 minute
    [SerializeField] private int difficultyChangeTime = 60;
    [SerializeField] private TextMeshProUGUI scoreText;

     //reference to Spawn Manager, trying to force it to load as it's not working for starting
     private SpawnManager spawn;
     //reference to the Player
     private PlayerController player;
     private GameObject gameOverUI;
     private int score = 0;

    void Start(){
        UpdateScore(0);
        gameOverUI = transform.Find("Game Over Page").gameObject;
        gameOverUI.SetActive(false);
        scoreText.gameObject.SetActive(false);
        StartGame();
    }
    //This runs after the Start() method of all the objects linked to, didn't work in Start()
    void Awake(){
        spawn = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        if(spawn == null || player == null){
            Debug.LogError("PLAYER OR SPAWN IS NULL TO GAME MANAGER");
        }
        //StartGame();
    }

    void Update(){

    }

    IEnumerator DifficultyChangeTimer(){
        yield return new WaitForSeconds(difficultyChangeTime);
        SetDifficulty(difficulty+1);
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
        else if(difficulty > 3){
            difficulty = 3;
            StopCoroutine("DifficultyChangeTimer");
        }
    }

    public void UpdateScore(int toAdd){
        score += toAdd;
        scoreText.text = "score: "+score;
    }

    public void GameOver(){
        gameOver = true;
        gameOverUI.SetActive(true);
        spawn.StopSpawning();
        player.DisablePlayer();
        SetDifficulty(1);
        StopCoroutine("DifficultyChangeTimer");
    }

    public void StartGame(){
        gameOver = false;
        scoreText.gameObject.SetActive(true);
        StartCoroutine("DifficultyChangeTimer");
        //I don't know why this makes everything stop working, 
        //only change is calling from here rather than from the 
        //Start() in each of the class definitions
        player.EnablePlayer(playerStrength,playerPower);
        spawn.RestartSpawn();
    }

    public void RestartGame(){
        gameOverUI.SetActive(false);
        gameOver = false;
        UpdateScore(-score);
        //enable player with 1000 strength and 5 power
        player.EnablePlayer(playerStrength,playerPower);
        spawn.RestartSpawn();
        StartGame();
    }
}