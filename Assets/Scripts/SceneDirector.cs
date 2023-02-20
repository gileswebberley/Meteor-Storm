using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GilesScoreSystem;

//attach this to a prefab and place in any scene you require it's functionality
//the game scene names can be added to the prefab - the leaderboard behaviour is a bit of a quick hack
public class SceneDirector : MonoBehaviour
{ 
    //this is untested except with a single entry and no scene changes at the moment
    [SerializeField] private string[] gameSceneName;

    void Awake()
    {
        //I want the leaderboard to print out so this is a kinda OnLoad() for the Leaderboard scene
        //must include a text area named LeaderboardTextArea
        if(SceneManager.GetActiveScene().name == TagManager.SCENE_LEADERBOARD) ScoringSystem.Instance.PrintLeaderboard(GameObject.Find(TagManager.LEADERBOARD_TEXT_AREA).GetComponent<TextMeshProUGUI>());
        //now check whether we're in a game-play scene - add the names to the SceneDirector prefab and place it in each scene
        //check the array of scene names that we should be managing as game play scenes
        if (Array.Exists(TagManager.GAME_SCENE_NAMES,element => element == SceneManager.GetActiveScene().name))
        {
            Debug.Log("We have entered a game play scene so game manager is setting up the components");
            //Now these are used to setup the game...
            GameManager.Instance.SetUpGame();
            GameManager.Instance.StartGame();
        }
        //sceneChanger += LoadSceneByName;
    }
    //for the start button on the sign up page to grab the name before going on to the game
    public void SignUp(TMP_InputField inputScoreName)
    {
        Debug.Log($"Sign up with the name {inputScoreName.text}");
        if(inputScoreName.text == "") return;//so it will create a guest score
        ScoringSystem.Instance.SetUpScoringName(inputScoreName.text);
        //can't have two properties in the editor it seems so doing this seperately
        //LoadSceneByButtonName(caller);
    }
    //for the Quit button in the game over page that is created by GameManager.
    //as I'm trying to attach it via AddListener I couldn't pass in the string,
    //had a quick go using a delegate but need to look into why that didn't help
    // public static void GoToStart()
    // {
    //     SceneManager.LoadScene("Welcome");
    // }

    public static void LoadSceneByName(string nameStr)
    {
        SceneManager.LoadScene(nameStr);
    }

    //created this so I just have to name the button the same as the scene name
    //and then pass it in as a parameter set in the Editor when it's attached to
    //the OnClick event. Used throughout the menu scenes
    public void LoadSceneByButtonName(GameObject caller)
    {
        Debug.Log($"Load scene named {caller.name}");
        SceneManager.LoadScene(caller.name);
    }
    
    //Quit function that works in both Editor and application attached to Quit button
    public static void QuitGame()
    {
        //leaderboard.SaveLeaderboard();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
