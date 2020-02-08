using UnityEngine;
using UnityHelpers;

public class SpawnFunctionsController : MonoBehaviour
{
    //public string enemiesPoolName;
    //private ObjectPool<Transform> enemiesPool;
    //private Transform arrowStartTransform;
    public OrbitCameraController arrowCamera;
    public Transform mainEnemyTarget;

    private void Start()
    {
        //enemiesPool = PoolManager.GetPool(enemiesPoolName);
        //arrowStartTransform = new GameObject("Arrow Start Position").transform;
    }

    public void OnBirdSpawned(Transform birdTransform, string poolName)
    {
        var bird = birdTransform.GetComponent<BirdController>();
        bird.Revive();
        bird.SetRandomSkin();
    }
    public void OnEnemySpawned(Transform enemyTransform, string poolName)
    {
        var enemy = enemyTransform.GetComponentInChildren<EnemyController>();
        var whenceYouCame = PoolManager.GetPool(poolName);
        enemy.Spawn(enemyTransform.position, enemyTransform.rotation, () =>
        {
            whenceYouCame.Return(enemy.transform);
        });

        enemy.enemyTarget = mainEnemyTarget; //Temporary, will find better solution
    }
    public void OnArrowSpawned(Transform arrowTransform, string poolName)
    {
        arrowCamera.target = arrowTransform;
        //arrowStartTransform.position = arrowTransform.position;
        //arrowCamera.ClearTargets();
        //arrowCamera.AddTarget(arrowStartTransform);
        //arrowCamera.AddTarget(arrowTransform);
    }
}
