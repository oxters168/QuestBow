using UnityEngine;
using UnityHelpers;

public class BowController : MonoBehaviour
{
    public Transform arrowPlacement;
    public Transform arrowFireSpot;
    public float minArrowDistanceSqr = 0.01f;
    public float maxPullDistance = 1;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.right, Color.blue);
    }

    public bool AtArrowPosition(Vector3 position)
    {
        return (position - arrowPlacement.position).sqrMagnitude < minArrowDistanceSqr;
    }
    public float PullAmount(Vector3 position)
    {
        var alignedPosition = position.ProjectPointToSurface(transform.position, transform.right);
        float distance = Vector3.Distance(arrowPlacement.position, alignedPosition);
        Vector3 direction = alignedPosition - arrowPlacement.position;
        if (Vector3.Dot(direction, Vector3.forward) >= 0)
            distance = 0;

        return Mathf.Clamp(distance, 0, maxPullDistance);
    }
}
