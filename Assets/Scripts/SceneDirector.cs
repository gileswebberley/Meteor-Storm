using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
   

    public void LoadSceneByButtonName(GameObject caller)
    {
        SceneManager.LoadScene(caller.name);
    }
}
