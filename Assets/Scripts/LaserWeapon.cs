using System.Collections;
using UnityEngine;
//using AudioManager to play our sounds
using GilesUtilities;

namespace GilesWeapons
{
    public class LaserWeapon : MonoBehaviour
    {
        //how much power each round has so the victims can check how much to remove from themselves
        //public int laserPower = 1; - now in laserBehaviour which is attached to the 'laser' object
        //number of rounds per full power load (so number of shots with max power)
        [RangeAttribute(0, 1000)] public int maxRounds = 100;
        //originally had this as maxPower so it would be 1 when on full power - deprecated
        //private float laserPowerDivisor;
        //lock used for Firing input control
        public bool bIsFiring { get; protected set; }
        //time step control for Firing input
        [SerializeField, RangeAttribute(0, 100)] protected int roundsPerSecond = 10;
        //the game object to be "fired"
        [SerializeField] protected GameObject laser;
        //an empty gameobject placeholder for where the lasers are fired
        [SerializeField] protected GameObject gunPosition;
        //by adding a sound in Editor it will play, in this case a firing noise
        [SerializeField] AudioClip firingSound;
        //AudioSource soundSource;

        void Awake()
        {
            bIsFiring = false;
            //if(firingSound != null) soundSource = gunPosition.AddComponent<AudioSource>();
        }

        public bool Fire()
        {
            if (!bIsFiring)
            {
                Debug.Log("FIRE!!!");
                bIsFiring = true;
                GameObject shot = Instantiate(laser, gunPosition.transform.position, gunPosition.transform.rotation);
                //adding sounds in, thought it was best to have the objects hold their own sounds and use a centralised system
                //learnt about using GetType(string type) to check if a "plugin-able" thing exists - ah, doesn't work :(
                //thanks to Numan KIZILIRMAK on StackOverflow for this handy way of making GetType() work, must be because
                //we're inside Unity I'm guessing -- this might be a way of using TagManager for component types? - not first off..
                string qualifiedName = typeof(AudioManager).AssemblyQualifiedName;
                if(firingSound != null && System.Type.GetType(qualifiedName) != null) AudioManager.PlaySoundFromPosition(gunPosition.transform.position, firingSound);
                //think we'll just leave it to be set in the Editor as it doesn't make sense to change it
                //shot.GetComponent<LaserBehaviour>().laserPower = laserPower;
                StartCoroutine(LimitShotsPerSecond());
                //let them know we've successfully fired our weapon
                return true;
            }
            else
            {
                //the weapon is already in a firing state and so has not been able to fire
                return false;
            }
        }

        //the coroutine to be run to limit firing to roundsPerSecond 
        protected IEnumerator LimitShotsPerSecond()
        {
            yield return new WaitForSeconds(1 / roundsPerSecond);
            bIsFiring = false;
        }
    }
}