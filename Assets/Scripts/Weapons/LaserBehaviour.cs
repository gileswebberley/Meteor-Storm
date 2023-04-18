using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GilesMovement;

//for collision detection so it holds it's own power variable - set when Instantiated in LaserWeapon
public class LaserBehaviour : MoveForwardRb
{
    public int laserPower;

    protected override void FixedUpdate()
    {
        //we want to change the direction of travel so don't run base.FixedUpdate();
        if(!RbAddForce(Vector3.back)){
            Destroy(gameObject);
        }
            
    }
}
