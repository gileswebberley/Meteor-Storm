using System.Collections;
using UnityEngine;
using IModels;

namespace OodModels
{
    public class LaserWeapon : MonoBehaviour
    {
        //how much power each round has so the victims can check how much to remove from themselves
        public int laserPower = 1;
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

        void Awake()
        {
            bIsFiring = false;
        }

        public bool Fire()
        {
            if (!bIsFiring)
            {
                Debug.Log("FIRE!!!");
                bIsFiring = true;
                GameObject shot = Instantiate(laser, gunPosition.transform.position, gunPosition.transform.rotation);
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