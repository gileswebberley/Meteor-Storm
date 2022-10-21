using System.Collections;
using System.Collections.Generic;

//Interface for basic Spawn Manager behaviour - this is not expected of interface
public interface ISpawnable
{
    public bool bIsSpawning {get;}
    int minSpawnAmount {get;}
    int maxSpawnAmount {get;}
    float maxSpawnX {get;}
    float maxSpawnY {get;}
    float minSpawnZ {get;}
    float maxSpawnZ {get;}
    void RestartSpawn();
    void StopSpawning();
    void SpawnAll();
}