using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A simple script which can be added to a game object so that it behaves as a button which changes scene
//to the name of the game object it's attached to. Originally developed in SceneDirector to be attached as
//an Onclick event in the inspector but felt a little cluncky
public class SceneChangeButton : MonoBehaviour
{
    Button myButton;
    void Awake()
    {
        if(!TryGetComponent<Button>(out myButton))
        { 
            Debug.LogError("SceneChangeButton is attached to a non button");
            myButton = gameObject.AddComponent<Button>();
        }
            myButton.onClick.AddListener(()=>
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(this.gameObject.name);
            });
    }
}
