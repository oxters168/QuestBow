using UnityEngine;

public class SpawnFunctionsController : MonoBehaviour
{
    public void OnBirdSpawned(Transform birdTransform)
    {
        var bird = birdTransform.GetComponent<BirdController>();
        bird.Revive();
        bird.SetRandomSkin();
    }
    public void OnEnemySpawned(Transform enemyTransform)
    {
        var enemy = enemyTransform.GetComponentInChildren<RootMotion.Dynamics.PuppetMaster>();
        //enemy.SwitchToKinematicMode();
        enemy.Resurrect();
        enemy.Rebuild();
        enemy.Teleport(enemyTransform.position, enemyTransform.rotation, true);
    }
}
