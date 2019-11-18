using UnityEngine;
using UnityHelpers;

public class BowController : MonoBehaviour
{
    public Transform arrowPlacement;
    public Transform arrowFireSpot;
    public float minArrowDistanceSqr = 0.01f;
    public float maxPullDistance = 1;
    public float maxLaunchSpeed = 200;
    public float stringMinStretch = 1, stringMaxStretch = 1.675f;

    public Transform topString, bottomString;
    //[Range(0, 1)]
    //public float testPercent;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.right, Color.blue);
        //SetPullPercent(testPercent);
    }

    public void SetPullPercent(float percent)
    {
        Vector3 pulledPosition = arrowPlacement.position - arrowPlacement.forward * maxPullDistance * percent;

        float adjacentLength = Vector3.Distance(arrowPlacement.position, topString.position);
        float hypotenuselength = Vector3.Distance(pulledPosition, topString.position);
        float stringAngle = Mathf.Acos(adjacentLength / hypotenuselength) * Mathf.Rad2Deg;

        float stretchMultiplier = (stringMaxStretch - stringMinStretch) * percent * percent + stringMinStretch;

        topString.localRotation = Quaternion.Euler(0, -stringAngle, 0);
        topString.localScale = Vector3.one * stretchMultiplier;

        bottomString.localRotation = Quaternion.Euler(0, stringAngle, 0);
        bottomString.localScale = Vector3.one * stretchMultiplier;
    }
    public bool AtArrowPosition(Vector3 position)
    {
        return (position - arrowPlacement.position).sqrMagnitude < minArrowDistanceSqr;
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
}
