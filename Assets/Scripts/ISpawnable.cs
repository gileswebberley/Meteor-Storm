using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IModels
{
    /*
    This is from my learning about implementing OOD into my coding. I started with 
    another one of my usual God Classes (SpawnManager) and tried to break it down.
    I made a lot of mistakes, the biggest being the fact that I put 'protected set;' 
    in the properties defined here. 
    POV - "THINK OF AN INTERFACE AS A CV FOR A PERSON WHO SPECIALISES IN
    THE SORT OF SKILLS NEEDED BY THE BASE CLASS (TYPE OF PERSON) AND USE THAT BASE
    CLASS TO IMPLEMENT THE INTERFACE AND THEN INHERIT FROM THAT BASE CLASS.
    REMEMBER THAT A CV IS NOT THE PLACE TO SAY WHAT YOU DON'T KNOW IT'S THE PLACE TO
     SAY WHAT YOU DO OFFER (it doesn't matter that classes outside of those implementing
     this interface know that the properties can only be set by subclasses!)"
    */


    //Interface for basic Spawn Manager behaviour
    public interface ISpawnable
    {
        public bool bIsSpawning { get; }
        public bool bHasStarted { get; }
        int minSpawnAmount { get; }
        int maxSpawnAmount { get; }
        int currentSpawnedEnemies { get; set; }
        float minSpawnX { get; }
        float maxSpawnX { get; }
        float minSpawnY { get; }
        float maxSpawnY { get; }
        float minSpawnZ { get; }
        float maxSpawnZ { get; }
        //++ perhaps SpawnEnemy and SpawnBonuses
        abstract void StartSpawn();
        abstract void RestartSpawn();
        abstract void StopSpawning();
        abstract void SpawnAll();
        abstract void SpawnEnemies();
        abstract void SpawnBonuses();
    }

    //as a way to track numbers without using FindObjectsOfType<MeteorBehaviour>
    public interface ISpawnedEnemy
    {
        //must have a reference to the spawn manager that will keep the count
        ISpawnable spawn { get; }
        //destroy game object when removed from spawn count
        abstract void RemoveFromSpawn();
        //create your reference to the ISpawnable in here
        abstract void AddToSpawn();
    }

    public interface IPlayer
    {
        float power { get; }
        float strength { get; }
        void EnablePlayer();

        void EnablePlayer(float power, float strength);
        void DisablePlayer();
    }

    public interface ILaser
    {
        //Weaponisation related items
        //how much power each round has so the victims can check how much to remove from themselves
        float laserPower { get; protected set; }
        //number of rounds per full power load (so number of shots with max power)
        float laserPowerUsageDivisor { get; protected set; }
        //originally had this as maxPower so it would be 1 when on full power - deprecated
        //private float laserPowerDivisor;
        //lock used for Firing input control
        bool bIsFiring { get; protected set; }
        //time step control for Firing input
        float roundsPerSecond { get; protected set; }
        //the game object to be "fired"
        GameObject laser { protected get; set; }
        //an empty gameobject placeholder for where the lasers are fired
        GameObject gunPosition { protected get; set; }
    }
}