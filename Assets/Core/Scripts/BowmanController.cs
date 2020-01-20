using UnityEngine;
using UnityHelpers;

public class BowmanController : MonoBehaviour
{
    public enum HandPose { relax, hold, pinch, }
    public enum Hand { none, left, right, }
    private Hand arrowHand, bowHand;
    public Animator leftHandAnimator, rightHandAnimator;
    public HandPose leftHandPose { get; private set; }
    public HandPose rightHandPose { get; private set; }

    public GameObject leftTouchController, rightTouchController;
    public GameObject leftHand, rightHand;

    public Transform bowTransform, arrowTransform;
    //public MimicTransform arrowPlaceholder;
    public Transform quiverBounds;
    public BowController bow;
    public OrbitCameraController arrowCamera;
    private bool arrowHeld, arrowInPlace;
    public float pullDistance;

    public Transform bowDock;
    public bool bowHeld;
    public bool canShoot;
    public event ArrowShotHandler onArrowShot;
    public delegate void ArrowShotHandler();

    public OculusInputController vrInput;

    public Vector3 arrowHandPosition, bowHandPosition;
    public Quaternion arrowHandRotation, bowHandRotation;

    public MeshRenderer quiverBoundsRenderer;
    public Transform bodyOrientation;
    public Vector3 leftHandPosition, rightHandPosition, headPosition, headForward;
    public Quaternion leftHandRotation, rightHandRotation;
    public bool leftTrigger, leftGrip, rightTrigger, rightGrip;
    public Vector2 watchAxes;

    public bool debugValues;

    public bool leftHandInQuiver { get; private set; }
    public bool rightHandInQuiver { get; private set; }

    public int totalArrowsFired { get; private set; }

    private bool vrErrored;

    private void Start()
    {
        ShowOrientation(false);
        OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.HighTop; //Will change where this is called to be more appropriate
    }
    private void Update()
    {
        RetrieveVRInput();
        HeavyCalculations();
        CalculateBodyOrientation();

        if (arrowHeld && arrowInPlace && ((arrowHand == Hand.left && !vrInput.leftTrigger) || (arrowHand == Hand.right && !vrInput.rightTrigger)) && canShoot)
        {
            totalArrowsFired++;
            bow.FireArrow();
            onArrowShot?.Invoke();
        }

        UpdateShownHands();
        UpdateBow();
        UpdatePlaceholderArrow();
        ArrowCameraControls();

        if (debugValues)
            DebugValues();
    }

    private void HeavyCalculations()
    {
        leftHandInQuiver = quiverBounds.IsPointInside(leftHandPosition);
        rightHandInQuiver = quiverBounds.IsPointInside(rightHandPosition);

        quiverBoundsRenderer.material.color = leftHandInQuiver || rightHandInQuiver ? Color.green : Color.red;
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
            vrErrored = false;

            headPosition = vrInput.thirdEyeTransform.position;
            headForward = vrInput.thirdEyeTransform.forward;
            leftHandPosition = vrInput.leftHandPosition;
            leftHandRotation = vrInput.leftHandRotation;
            rightHandPosition = vrInput.rightHandPosition;
            rightHandRotation = vrInput.rightHandRotation;
            leftTrigger = vrInput.leftTrigger;
            leftGrip = vrInput.leftGrip;
            rightTrigger = vrInput.rightTrigger;
            rightGrip = vrInput.rightGrip;
            watchAxes = vrInput.leftStickValue;
        }
        else if (!vrErrored)
        {
            Debug.LogError("BowmanController: No vrinput given");
            vrErrored = true;
        }
    }

    public void ShowDebugValues(bool onOff)
    {
        debugValues = onOff;
    }
    private void DebugValues()
    {
        DebugPanel.Log("RH Position", arrowHandPosition);
        DebugPanel.Log("Arrow Held", arrowHeld);
        DebugPanel.Log("Arrow In Place", arrowInPlace);
        DebugPanel.Log("Pull Distance", pullDistance);
    }

    private void UpdateShownHands()
    {
        bool showLeftAsHand = arrowHand == Hand.left || bowHand == Hand.left;
        bool showRightAsHand = arrowHand == Hand.right || bowHand == Hand.right;
        leftHand.SetActive(showLeftAsHand);
        rightHand.SetActive(showRightAsHand);
        leftTouchController.SetActive(!showLeftAsHand);
        rightTouchController.SetActive(!showRightAsHand);

        leftHandPose = arrowHand == Hand.left ? HandPose.pinch : (bowHand == Hand.left ? HandPose.hold : HandPose.relax);
        rightHandPose = arrowHand == Hand.right ? HandPose.pinch : (bowHand == Hand.right ? HandPose.hold : HandPose.relax);

        leftHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.hold), leftHandPose == HandPose.hold);
        leftHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.pinch), leftHandPose == HandPose.pinch);
        leftHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.relax), leftHandPose == HandPose.relax);

        rightHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.hold), rightHandPose == HandPose.hold);
        rightHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.pinch), rightHandPose == HandPose.pinch);
        rightHandAnimator.SetBool(GetHandPoseTriggerName(HandPose.relax), rightHandPose == HandPose.relax);

        leftHand.transform.position = leftHandPosition;
        leftHand.transform.rotation = leftHandRotation;
        leftTouchController.transform.position = leftHandPosition;
        leftTouchController.transform.rotation = leftHandRotation;
        rightHand.transform.position = rightHandPosition;
        rightHand.transform.rotation = rightHandRotation;
        rightTouchController.transform.position = rightHandPosition;
        rightTouchController.transform.rotation = rightHandRotation;
    }
    public void SetLeftHandPose(HandPose pose)
    {
        leftHandPose = pose;
        //leftHandAnimator.SetTrigger(GetHandPoseTriggerName(pose));
    }
    public void SetRightHandPose(HandPose pose)
    {
        rightHandPose = pose;
        //rightHandAnimator.SetTrigger(GetHandPoseTriggerName(pose));
    }
    private string GetHandPoseTriggerName(HandPose pose)
    {
        string poseName;
        if (pose == HandPose.hold)
            poseName = "Hold";
        else if (pose == HandPose.pinch)
            poseName = "Pinch";
        else
            poseName = "Relax";

        return poseName;
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

    private void UpdateBow()
    {
        if ((bowHand == Hand.left && !leftGrip) || (bowHand == Hand.right && !rightGrip))
        {
            bowHeld = false;
            bowHand = Hand.none;
        }

        if (!bowHeld)
        {
            if (leftGrip && leftHandInQuiver)
            {
                bowHeld = true;
                bowHand = Hand.left;
            }
            else if (rightGrip && rightHandInQuiver)
            {
                bowHeld = true;
                bowHand = Hand.right;
            }
        }

        if (bowHand == Hand.left)
        {
            bowHandPosition = leftHandPosition;
            bowHandRotation = leftHandRotation;
        }
        else if (bowHand == Hand.right)
        {
            bowHandPosition = rightHandPosition;
            bowHandRotation = rightHandRotation;
        }

        if (bowHeld)
        {
            bowTransform.position = bowHandPosition;
            bowTransform.rotation = bowHandRotation;
        }
        else
        {
            bowTransform.position = bowDock.position;
            bowTransform.rotation = bowDock.rotation;
        }
    }
    private void UpdatePlaceholderArrow()
    {
        if ((arrowHand == Hand.left && !leftTrigger) || (arrowHand == Hand.right && !rightTrigger))
        {
            arrowHeld = false;
            arrowHand = Hand.none;
        }

        arrowTransform.gameObject.SetActive(arrowHeld && canShoot);

        if (arrowHand == Hand.left)
        {
            arrowHandPosition = leftHandPosition;
            arrowHandRotation = leftHandRotation;
        }
        else if (arrowHand == Hand.right)
        {
            arrowHandPosition = rightHandPosition;
            arrowHandRotation = rightHandRotation;
        }

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
                    arrowTransform.rotation = bow.arrowPlacement.rotation;
                }
            }
            else
            {
                arrowTransform.position = arrowHandPosition;
                arrowTransform.rotation = arrowHandRotation;
            }

            if (pullDistance <= 0)
                arrowInPlace = false;

            quiverBoundsRenderer.material.color = Color.white;
        }
        else
        {
            arrowInPlace = false;
            pullDistance = 0;

            if (leftTrigger && leftHandInQuiver)
            {
                arrowHeld = true;
                arrowHand = Hand.left;
            }
            else if (rightTrigger && rightHandInQuiver)
            {
                arrowHeld = true;
                arrowHand = Hand.right;
            }
        }

        bow.SetPullPercent(pullDistance / bow.maxPullDistance);
    }
}
