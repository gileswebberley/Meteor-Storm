using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for collision detection so it holds it's own power variable - set when Instantiated in LaserWeapon
public class LaserBehaviour : MoveForwardRb
{
    public int laserPower;

    protected override void FixedUpdate()
    {//this checks that the GameObject will add/remove itself from the counter
        //we want to change the death behaviour so don't run base.FixedUpdate();
        RbAddForce(Vector3.back);//means we're out of GameBounds.z//this checks that the GameObject will add/remove itself from the counter
            
    }
}
