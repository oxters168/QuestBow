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

    public OculusInputController vrInput;

    public MeshRenderer quiverBoundsRenderer;
    public Transform bodyOrientation;
    public Vector3 arrowHandPosition, bowHandPosition, headPosition, headForward;
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
        RetrieveVRInput();
        CalculateBodyOrientation();

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

    public void ShowOrientation(bool onOff)
    {
        quiverBoundsRenderer.enabled = onOff;
        bodyOrientation.gameObject.SetActive(onOff);
    }
    private void RetrieveVRInput()
    {
        if (vrInput != null)
        {
            headPosition = vrInput.thirdEyeTransform.position;
            headForward = vrInput.thirdEyeTransform.forward;
            arrowHandPosition = vrInput.dominantHandPosition;
            arrowHandRotation = vrInput.dominantHandRotation;
            bowHandPosition = vrInput.secondaryHandPosition;
            bowHandRotation = vrInput.secondaryHandRotation;
            holdArrow = vrInput.primaryTriggerValue > 0;
            watchAxes = vrInput.secondaryStickValue;
        }
    }

    private void CalculateBodyOrientation()
    {
        Vector3 headPlanarForward = headForward.Planar(Vector3.up);
        Vector3 bodyRight = Vector3.Cross(Vector3.up, headPlanarForward);
        //Vector3 calculatedForward = headPlanarForward;

        Vector3 bowHandDirection = (bowHandPosition - headPosition).normalized;
        Vector3 bowHandPlanarDirection = bowHandDirection.Planar(Vector3.up);
        float bowHandPlanarPercent = 1 - Vector3.Angle(bowHandDirection, bowHandPlanarDirection) / 90;
        Vector3 arrowHandDirection = (arrowHandPosition - headPosition).normalized;
        Vector3 arrowHandPlanarDirection = arrowHandDirection.Planar(Vector3.up);
        float arrowHandPlanarPercent = 1 - Vector3.Angle(arrowHandDirection, arrowHandPlanarDirection) / 90;

        bodyOrientation.position = headPosition - new Vector3(0, headPosition.y / 3, 0);
        //calculatedForward = (VectorHelpers.Average(bowHandPosition, arrowHandPosition) - bodyOrientation.position).Planar(Vector3.up);
        Vector3 calculatedForward = ((headPlanarForward + bowHandPlanarDirection * bowHandPlanarPercent + arrowHandPlanarDirection * arrowHandPlanarPercent) / 3).normalized;
        bodyOrientation.forward = calculatedForward;
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
