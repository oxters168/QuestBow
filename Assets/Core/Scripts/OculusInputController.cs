using UnityEngine;

public class OculusInputController : MonoBehaviour
{
    public BowmanController bowman;

    public Transform controllersParent;

    public bool backButtonValue, backButtonProcessed;
    public Vector3 dominantHandPosition, secondaryHandPosition;
    public Quaternion dominantHandRotation, secondaryHandRotation;
    public float primaryTriggerValue, secondaryGripValue;
    public Vector2 secondaryStickValue;
    public OVRInput.Handedness dominantHand = OVRInput.Handedness.RightHanded;

    void Update()
    {
        GetInput();
        ControlBowman();

        if (backButtonValue && !backButtonProcessed)
        {
            //ondown
            SceneController.ShowMenu(!SceneController.menuShown);
            backButtonProcessed = true;
        }
        else if (!backButtonValue && backButtonProcessed)
        {
            //onup
            backButtonProcessed = false;
        }
    }

    private void GetInput()
    {
        primaryTriggerValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis1D.RIndexTrigger : OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.All);
        secondaryGripValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis1D.LHandTrigger : OVRInput.RawAxis1D.RHandTrigger, OVRInput.Controller.All);

        secondaryStickValue = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawAxis2D.LThumbstick : OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.All);
        backButtonValue = OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.All);
        //secondaryLowerButton = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawButton.X : OVRInput.RawButton.A, OVRInput.Controller.All);
        //secondaryHigherButton = OVRInput.Get(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.RawButton.Y : OVRInput.RawButton.B, OVRInput.Controller.All);

        dominantHandPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch));
        dominantHandRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch);

        secondaryHandPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch));
        secondaryHandRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(dominantHand == OVRInput.Handedness.RightHanded ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch);
    }
    private void ControlBowman()
    {
        bowman.arrowHandPosition = dominantHandPosition;
        bowman.arrowHandRotation = dominantHandRotation;
        bowman.bowHandPosition = secondaryHandPosition;
        bowman.bowHandRotation = secondaryHandRotation;
        bowman.holdArrow = primaryTriggerValue > 0;
        bowman.watchAxes = secondaryStickValue;
    }
}
