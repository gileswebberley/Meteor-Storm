using UnityEngine;


public class RandomSpeedMoveForwardsRb : MoveForwardRb
{
    //private field with access for Unity Editor Inspector
    [SerializeField] private float speedRandomiserRange = 7f;

    protected override void Start()
    {
        base.Start();
        speed = Random.Range(speed/speedRandomiserRange,speed*speedRandomiserRange);
    }
}
