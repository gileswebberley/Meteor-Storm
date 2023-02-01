using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GilesUtilities
{
public class AudioManager : MonoBehaviour
{

    public static void PlaySoundFromPosition(Vector3 pos, AudioClip clip)
    {
        //create a new Game object to hold an AudioSOurce component
        GameObject moveableSource = new GameObject();
        //moveableSource.transform.SetParent(this.gameObject)
        AudioSource audioPlayer = moveableSource.AddComponent<AudioSource>();
        audioPlayer.spatialBlend = 0.75f;
        //move into position
        moveableSource.transform.position = pos;
        audioPlayer.PlayOneShot(clip);
        //now set up the game object to be destroyed when the clip has played
        Destroy(moveableSource,clip.length);
    }
}
}