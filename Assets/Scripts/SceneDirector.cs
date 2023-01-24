using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneDirector : MonoBehaviour
{ 
    void Awake()
    {
        //I want the leaderboard to print out so this is a kinda OnLoad() for the Leaderboard scene
        //must include a text area named LeaderboardTextArea
        if(SceneManager.GetActiveScene().name == "Leaderboard") ScoringSystem.Instance.PrintLeaderboard(GameObject.Find("LeaderboardTextArea").GetComponent<TextMeshProUGUI>());
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
    public static void GoToStart()
    {
        SceneManager.LoadScene("Welcome");
    }

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
