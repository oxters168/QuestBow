using UnityEngine;
using UnityHelpers;

public class BowmanController : MonoBehaviour
{
    public ArrowController arrowPrefab;
    public Transform arrowsParent;
    public ArrowController[] arrowsPool = new ArrowController[5];
    private int arrowPoolIndex;
    //private ObjectPool<ArrowController> arrowsPool;

    public MimicTransform arrowPlaceholder;
    public Transform quiverBounds;
    public BowController bow;
    private bool arrowHeld, arrowInPlace;
    public float pullDistance;

    public Transform controllersParent;
    public Vector3 rightHandControllerPosition;
    public float primaryTriggerValue, secondaryGripValue;
    public OVRInput.Handedness dominantHand = OVRInput.Handedness.RightHanded;

    //private void Start()
    //{
    //    arrowsPool = new ObjectPool<ArrowController>(arrowPrefab, 5, true, arrowsParent);
    //}
    private void Update()
    {
        GetInput();

        if (arrowHeld && arrowInPlace && primaryTriggerValue <= 0)
            FireArrow();

        UpdatePlaceholderArrow();

        DebugPanel.Log("RH Position", rightHandControllerPosition);
        DebugPanel.Log("Primary Trigger", primaryTriggerValue);
        DebugPanel.Log("Arrow Held", arrowHeld);
        DebugPanel.Log("Arrow In Place", arrowInPlace);
        DebugPanel.Log("Pull Distance", pullDistance);
    }

    private void GetInput()
    {
        primaryTriggerValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis1D.RIndexTrigger : OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.All);
        secondaryGripValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis1D.LHandTrigger : OVRInput.RawAxis1D.RHandTrigger, OVRInput.Controller.All);

        rightHandControllerPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch));
        //controllerRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }

    private void UpdatePlaceholderArrow()
    {
        if (primaryTriggerValue <= 0)
            arrowHeld = false;

        arrowPlaceholder.enabled = pullDistance <= 0;
        arrowPlaceholder.gameObject.SetActive(arrowHeld);

        if (arrowHeld)
        {
            if (bow.AtArrowPosition(arrowPlaceholder.transform.position))
                arrowInPlace = true;
            Vector3 alignedPosition = Vector3.zero;
            pullDistance = 0;
            if (arrowInPlace)
            {
                pullDistance = bow.PullAmount(rightHandControllerPosition);
                if (pullDistance > 0)
                {
                    alignedPosition = bow.arrowPlacement.position - bow.arrowPlacement.forward * pullDistance;
                    arrowPlaceholder.transform.position = alignedPosition;
                    arrowPlaceholder.transform.forward = bow.transform.forward;
                }
            }

            if (pullDistance <= 0)
                arrowInPlace = false;
        }
        else
        {
            arrowInPlace = false;
            pullDistance = 0;

            arrowHeld = primaryTriggerValue > 0 && quiverBounds.GetTotalBounds(true).Contains(rightHandControllerPosition);
        }

        bow.SetPullPercent(pullDistance / bow.maxPullDistance);
    }

    private void FireArrow()
    {
        GetArrow((arrow) =>
        {
            Vector3 fireVelocity = bow.arrowFireSpot.forward * bow.maxLaunchSpeed * (pullDistance / bow.maxPullDistance);
            arrow.Translate(bow.arrowFireSpot.position, bow.arrowFireSpot.rotation, fireVelocity);
        });
        /*arrowsPool.Get((arrow) =>
        {
            Vector3 fireVelocity = bow.arrowFireSpot.forward * bow.maxLaunchSpeed * (pullDistance / bow.maxPullDistance);
            StartCoroutine(arrow.Translate(bow.arrowFireSpot.position, bow.arrowFireSpot.rotation, fireVelocity));
        });*/
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
