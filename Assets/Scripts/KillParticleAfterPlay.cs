using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GilesUtilities {
//to destroy any particle systems that are Instantiated
public class KillParticleAfterPlay : MonoBehaviour
{
    void Start()
    {
        //get a reference to the instance we are attached to
        ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
        //access the particle system's settings to find how long it lasts, then kill it after that long
        Destroy(gameObject,(ps.main.duration+ps.main.startLifetimeMultiplier)*ps.main.simulationSpeed);
    }
}
}
