namespace GilesSpawnSystem
{
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
}