using UnityEngine;
using UnityHelpers;

public class BowController : MonoBehaviour
{
    public ArrowController arrowPrefab;
    public ArrowController[] arrowsPool = new ArrowController[5];
    private int arrowPoolIndex;

    public Transform arrowPlacement;
    public Transform arrowFireSpot;
    public float minArrowDistanceSqr = 0.01f;
    public float maxPullDistance = 1;
    public float maxLaunchSpeed = 200;
    public float stringMinStretch = 1, stringMaxStretch = 1.675f;

    public float pullPercent { get; private set; }

    public Transform topString, bottomString;

    public PoolSpawner.SpawnEvent onArrowSpawn;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.right, Color.blue);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(arrowPlacement.position, minArrowDistanceSqr * minArrowDistanceSqr);
    }

    public void SetPullPercent(float percent)
    {
        pullPercent = Mathf.Clamp01(percent);

        Vector3 pulledPosition = arrowPlacement.position - arrowPlacement.forward * maxPullDistance * pullPercent;

        float adjacentLength = Vector3.Distance(arrowPlacement.position, topString.position);
        float hypotenuselength = Vector3.Distance(pulledPosition, topString.position);
        float stringAngle = Mathf.Acos(adjacentLength / hypotenuselength) * Mathf.Rad2Deg;

        float stretchMultiplier = (stringMaxStretch - stringMinStretch) * pullPercent * pullPercent + stringMinStretch;

        topString.localRotation = Quaternion.Euler(0, -stringAngle, 0);
        topString.localScale = Vector3.one * stretchMultiplier;

        bottomString.localRotation = Quaternion.Euler(0, stringAngle, 0);
        bottomString.localScale = Vector3.one * stretchMultiplier;
    }
    public bool AtArrowPosition(Vector3 position)
    {
        var distanceSqr = (position - arrowPlacement.position).sqrMagnitude;
        DebugPanel.Log("Distance sqr from arrow to bow", distanceSqr);
        return distanceSqr <= minArrowDistanceSqr;
    }
    public float PullAmount(Vector3 position)
    {
        var alignedPosition = position.ProjectPointToSurface(arrowPlacement.position, arrowPlacement.right);
        float distance = Vector3.Distance(arrowPlacement.position, alignedPosition);
        Vector3 direction = alignedPosition - arrowPlacement.position;
        if (Vector3.Dot(direction, arrowPlacement.forward) >= 0)
            distance = 0;

        return Mathf.Clamp(distance, 0, maxPullDistance);
    }
    public void FireArrow()
    {
        GetArrow((arrow) =>
        {
            Vector3 fireVelocity = arrowFireSpot.forward * maxLaunchSpeed * pullPercent;
            arrow.Translate(arrowFireSpot.position, arrowFireSpot.rotation, fireVelocity);
            onArrowSpawn?.Invoke(arrow.transform);
        });
    }
    private void GetArrow(System.Action<ArrowController> onGot)
    {
        if (arrowsPool[arrowPoolIndex] != null)
            Destroy(arrowsPool[arrowPoolIndex].gameObject);

        arrowsPool[arrowPoolIndex] = Instantiate(arrowPrefab);
        onGot(arrowsPool[arrowPoolIndex]);
        arrowPoolIndex = (arrowPoolIndex + 1) % arrowsPool.Length;
    }
}
