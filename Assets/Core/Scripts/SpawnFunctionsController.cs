using UnityEngine;
using UnityHelpers;

public class SpawnFunctionsController : MonoBehaviour
{
    public Transform mainEnemyTarget;

    public void OnBirdSpawned(Transform birdTransform)
    {
        var bird = birdTransform.GetComponent<BirdController>();
        bird.Revive();
        bird.SetRandomSkin();
    }
    public void OnEnemySpawned(Transform enemyTransform)
    {
        var enemy = enemyTransform.GetComponentInChildren<EnemyController>();
        enemy.puppet.Resurrect();
        enemy.puppet.Rebuild();
        enemy.puppet.Teleport(enemyTransform.position, enemyTransform.rotation, true);
        enemy.target = mainEnemyTarget; //Temporary, will find better solution
    }
}
