using UnityEngine;
using UnityHelpers;

public class BowmanController : MonoBehaviour
{
    public Transform bowTransform, arrowTransform;
    //public MimicTransform arrowPlaceholder;
    public Transform quiverBounds;
    public BowController bow;
    public OrbitCameraController arrowCamera;
    private bool arrowHeld, arrowInPlace;
    public float pullDistance;

    public bool canShoot;
    public event ArrowShotHandler onArrowShot;
    public delegate void ArrowShotHandler();

    public Vector3 arrowHandPosition, bowHandPosition;
    public Quaternion arrowHandRotation, bowHandRotation;
    public bool holdArrow;
    public Vector2 watchAxes;

    public int totalArrowsFired { get; private set; }

    private void Start()
    {
        OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.HighTop; //Will change where this is called to be more appropriate
    }
    private void Update()
    {
        if (arrowHeld && arrowInPlace && !holdArrow && canShoot)
        {
            totalArrowsFired++;
            bow.FireArrow();
            onArrowShot?.Invoke();
        }

        bowTransform.position = bowHandPosition;
        bowTransform.rotation = bowHandRotation;
        UpdatePlaceholderArrow();
        ArrowCameraControls();

        DebugPanel.Log("RH Position", arrowHandPosition);
        DebugPanel.Log("Primary Trigger", holdArrow);
        DebugPanel.Log("Arrow Held", arrowHeld);
        DebugPanel.Log("Arrow In Place", arrowInPlace);
        DebugPanel.Log("Pull Distance", pullDistance);
    }

    public void DestroyAllArrows()
    {
        bow.DestroyAllArrows();
    }

    private void ArrowCameraControls()
    {
        arrowCamera.SetLookHorizontal(watchAxes.x);
        arrowCamera.SetPush(watchAxes.y);
        //arrowCamera.SetPush(secondaryLowerButton ? -1 : (secondaryHigherButton ? 1 : 0));
    }
    private void UpdatePlaceholderArrow()
    {
        if (!holdArrow)
            arrowHeld = false;

        //arrowPlaceholder.enabled = pullDistance <= 0;
        arrowTransform.gameObject.SetActive(arrowHeld && canShoot);

        if (arrowHeld)
        {
            if (bow.AtArrowPosition(arrowHandPosition))
                arrowInPlace = true;
            Vector3 alignedPosition = Vector3.zero;
            pullDistance = 0;
            if (arrowInPlace)
            {
                pullDistance = bow.PullAmount(arrowHandPosition);
                if (pullDistance > 0)
                {
                    alignedPosition = bow.arrowPlacement.position - bow.arrowPlacement.forward * pullDistance;
                    arrowTransform.position = alignedPosition;
                    arrowTransform.forward = bow.transform.forward;
                }
            }
            else
            {
                arrowTransform.position = arrowHandPosition;
                arrowTransform.rotation = arrowHandRotation;
            }

            if (pullDistance <= 0)
                arrowInPlace = false;
        }
        else
        {
            arrowInPlace = false;
            pullDistance = 0;

            arrowHeld = holdArrow && quiverBounds.IsPointInside(arrowHandPosition);
        }

        bow.SetPullPercent(pullDistance / bow.maxPullDistance);
    }
}
