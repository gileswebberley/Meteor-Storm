using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorBehaviour : MonoBehaviour
{
    public float power = 2f;
    private float startPower;
    //when we are destroyed play this particle system in our position
    public ParticleSystem explodePS;
    //when we're hit check how powerful the laser is that hit us by checking player
    private PlayerController player;
    //so we can add our score via the GameManager
    private GameManager gameHQ;

    // Start is called before the first frame update
    void Start()
    {
        //just for testing
        //Instantiate(explodePS,transform.position, transform.rotation);
        //for scoring
        startPower = power;
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameHQ = GameObject.Find("Game Manager").GetComponent<GameManager>();
        //add a bit of random rotation on birth
        GetComponent<Rigidbody>().AddTorque(Random.Range(-5,5),Random.Range(-5,5),Random.Range(-5,5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if(other.CompareTag("LaserShot")){
            //hit by a laser, check how powerful it is
            power -= player.GetLaserPower();
            //remove laser shot that hit
            Destroy(other.gameObject);
            if(power <= 0){
                //can't get the particle system to work :( ++ There we go, simply use Instantiate rather than try to play it
                Instantiate(explodePS,transform.position, transform.rotation);
                //destroyed
                Destroy(gameObject);
                //add my original power to the player score
                gameHQ.UpdateScore(((int)startPower));//warning casting to int
            }
        }
    }
}
