using UnityEngine;
using UnityHelpers;

public class BirdsBringer : MonoBehaviour
{
    private ObjectPool<Transform> birdsPool;
    //public Rect spawnArea;
    public Bounds spawnArea;
    public bool spawnBirds;
    public float spawnMinRate = 2, spawnMaxRate = 5;
    private float prevSpawnTime, currentSpawnTime;

    private void Start()
    {
        birdsPool = PoolManager.GetPool("BirdsPool");
        SetNextSpawnTime();
    }
    private void Update()
    {
        if (spawnBirds && Time.time - prevSpawnTime >= currentSpawnTime)
        {
            var bird = birdsPool.Get<BirdController>();
            bird.Revive();
            bird.SetRandomSkin();
            bird.transform.forward = transform.forward;
            bird.transform.position = GetRandomSpawnPoint();
            SetNextSpawnTime();
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 startCorner = transform.position - transform.forward * spawnArea.size.z / 2 - transform.right * spawnArea.size.x / 2 - transform.up * spawnArea.size.y / 2;
        float randomRight = Random.Range(0, spawnArea.size.x);
        float randomUp = Random.Range(0, spawnArea.size.y);
        return startCorner + transform.right * randomRight + transform.up * randomUp;
    }
    private void SetNextSpawnTime()
    {
        prevSpawnTime = Time.time;
        currentSpawnTime = Random.Range(spawnMinRate, spawnMaxRate);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var cubeMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        var oldMatrix = Gizmos.matrix;
        Gizmos.matrix *= cubeMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnArea.size.x, spawnArea.size.y, spawnArea.size.z));
        Gizmos.matrix = oldMatrix;
        Vector3 startCorner = transform.position - transform.forward * spawnArea.size.z / 2 - transform.right * spawnArea.size.x / 2 - transform.up * spawnArea.size.y / 2;
        Gizmos.DrawSphere(startCorner, 1);
    }
}
