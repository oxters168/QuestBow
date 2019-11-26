using UnityEngine;
using UnityHelpers;

public class BowmanController : MonoBehaviour
{
    public MimicTransform arrowPlaceholder;
    public Transform quiverBounds;
    public BowController bow;
    public OrbitCameraController arrowCamera;
    private bool arrowHeld, arrowInPlace;
    public float pullDistance;

    public Transform controllersParent;
    public Vector3 rightHandControllerPosition;
    public float primaryTriggerValue, secondaryGripValue;
    public Vector2 secondaryStickValue;
    //public bool secondaryLowerButton, secondaryHigherButton;
    public OVRInput.Handedness dominantHand = OVRInput.Handedness.RightHanded;

    private void Update()
    {
        GetInput();

        if (arrowHeld && arrowInPlace && primaryTriggerValue <= 0)
            bow.FireArrow();

        UpdatePlaceholderArrow();
        ArrowCameraControls();

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

        secondaryStickValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis2D.LThumbstick : OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.All);
        //secondaryLowerButton = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawButton.X : OVRInput.RawButton.A, OVRInput.Controller.All);
        //secondaryHigherButton = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawButton.Y : OVRInput.RawButton.B, OVRInput.Controller.All);

        rightHandControllerPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch));
        //controllerRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }

    private void ArrowCameraControls()
    {
        arrowCamera.SetLookHorizontal(secondaryStickValue.x);
        arrowCamera.SetPush(secondaryStickValue.y);
        //arrowCamera.SetPush(secondaryLowerButton ? -1 : (secondaryHigherButton ? 1 : 0));
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
}
