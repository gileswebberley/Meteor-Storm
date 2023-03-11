using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a static class to centralise all the string literals that are used throughout the game
public static class TagManager
{
    //this is based on an article I read and seemed like a nice clean solution
    //to centralise the strings that are used in Find(), CompareTag(), and Resources.Load()
    //player prefab name with a PlayerController component
    public const string PLAYER = "Player";
    public const string PLAYER_SPEED_PROPERTY = "Player Speed";
    //spawn manager prefab name with a ISpawnable component
    public const string SPAWN = "Spawn Manager";
    //used in Game Manager for dynamically creating the game over screen
    public const string GAMEOVERUI = "Game Over Page";
    //text area where the ranking message is shown in game over
    public const string GAMEOVERSCORETEXT = "Score Ranking";
    //button for restarting the game in game over page
    public const string GAMEOVERRESTARTBUTTON = "Restart";
    //button for quitting the game in game over page
    public const string GAMEOVERQUITBUTTON = "Quit";
    //tags that are set on bonuses
    public const string AMMOBONUS = "ChargeUp";
    public const string HEALTHBONUS = "StrengthUp";
    //tags for the various enemies - could just change all of their tags to Enemy?
    public const string ENEMY1 = "Planet";
    public const string ENEMY2 = "Meteor";
    public const string ENEMY3 = "Star";
    //name of the laser shot prefab
    public const string BULLET = "LaserShot";

    //SceneDirector has a kinda OnLoad functionality for filling a text area within the Leaderboard scene
    public const string SCENE_LEADERBOARD = "Leaderboard";
    public const string LEADERBOARD_TEXT_AREA = "LeaderboardTextArea";
    //the name of any scene that is the game itself
    //learnt that arrays have to be redonly rather than const because the value is not initialised until run-time
    public static readonly string[] GAME_SCENE_NAMES = new string[] {"MeteorStorm"};

    public const string AUDIO_MANAGER_SOURCE = "AudioSourceObject";

    //just a test for using in GetComponent() not sure this even makes logical sense!?
    //testing whether we can use this in combination with System.Types.GetType() - not currently
    //public static string PLAYER_TYPE = typeof(PlayerController).AssemblyQualifiedName;
    //just created this in an other project and thought this might be a good place to stash it
    //Safely try to Find(find) and GetComponent<T>
    public static bool SafeFind<T>(string find, ref T component)
    {
        GameObject returnGO = GameObject.Find(find);
        if(returnGO == null){
            Debug.Log($"Find({find}) returned null");
             component = default(T);
             return false;
        }else{
            Debug.Log($"Find({find}) returned a game object");
            if(returnGO.TryGetComponent<T>(out T tmpComponent)){
                component = tmpComponent;
                return true;
            }else{
                Debug.Log("But couldn't find the component");
                component = default(T);
                return false;
            }
        }
    }
}
