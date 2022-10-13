using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//to destroy any particle systems that are Instantiated
public class KillParticleAfterPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
        //access the particle system's settings to find how long it lasts, then kill it after that long
        Destroy(gameObject,(ps.main.duration+ps.main.startLifetimeMultiplier)*ps.main.simulationSpeed);
    }
}
