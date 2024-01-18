using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GilesMovement;

//for collision detection so it holds it's own power variable - set when Instantiated in LaserWeapon
public class LaserBehaviour : MoveForwardRb
{
    public int laserPower;
    //going to implement ray cast collision detection and move it out of the enemies
    protected Vector3 lastPosition;
    protected Vector3 displacement;
    protected Vector3 direction = Vector3.back;
    protected LayerMask hitLayerMask;

    void Start()
    {
        base.Start();
        lastPosition = transform.position;
        hitLayerMask |= (1 << LayerMask.NameToLayer("Enemy"));
    }

    protected override void FixedUpdate()
    {
        //we want to change the direction of travel so don't run base.FixedUpdate();
        if (!RbAddForce(direction))
        {
            Destroy(gameObject);
        }
        displacement = transform.position - lastPosition;
        lastPosition = transform.position;
        Ray ray = new Ray(transform.position, displacement);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray,out raycastHit,displacement.magnitude,hitLayerMask))
        {
            Debug.Log(this.name + " has hit " + raycastHit.collider.name + " at point " + raycastHit.point);
            raycastHit.collider.SendMessage("OnProjectileCollision", this, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
            
    }
}
