using UnityEngine;
using UnityHelpers;

public class OculusInputController : MonoBehaviour
{
    public Transform controllersParent, thirdEyeTransform;

    #region Raw Values
    public Vector3 rightHandPosition;
    public Vector3 leftHandPosition;
    public Quaternion rightHandRotation;
    public Quaternion leftHandRotation;
    public bool backButton;
    public bool aButton;
    public bool bButton;
    public bool xButton;
    public bool yButton;
    public bool l3Button;
    public bool r3Button;
    public float rightTriggerValue;
    public float rightGripValue;
    public float leftTriggerValue;
    public float leftGripValue;
    public Vector2 leftStickValue;
    public Vector2 rightStickValue;
    #endregion

    #region Processed Values
    public bool backButtonDown;
    public bool backButtonUp;
    public bool backButtonTriggered;

    public bool aButtonDown;
    public bool aButtonUp;
    public bool aButtonTriggered;

    public bool bButtonDown;
    public bool bButtonUp;
    public bool bButtonTriggered;

    public bool xButtonDown;
    public bool xButtonUp;
    public bool xButtonTriggered;

    public bool yButtonDown;
    public bool yButtonUp;
    public bool yButtonTriggered;

    public bool l3ButtonDown;
    public bool l3ButtonUp;
    public bool l3ButtonTriggered;

    public bool r3ButtonDown;
    public bool r3ButtonUp;
    public bool r3ButtonTriggered;

    [Range(0, 1)]
    public float leftTriggerThreshold = 0.01f;
    public bool leftTrigger;
    public bool leftTriggerDown;
    public bool leftTriggerUp;
    public bool leftTriggerTriggered;

    [Range(0, 1)]
    public float rightTriggerThreshold = 0.01f;
    public bool rightTrigger;
    public bool rightTriggerDown;
    public bool rightTriggerUp;
    public bool rightTriggerTriggered;

    [Range(0, 1)]
    public float leftGripThreshold = 0.01f;
    public bool leftGrip;
    public bool leftGripDown;
    public bool leftGripUp;
    public bool leftGripTriggered;

    [Range(0, 1)]
    public float rightGripThreshold = 0.01f;
    public bool rightGrip;
    public bool rightGripDown;
    public bool rightGripUp;
    public bool rightGripTriggered;
    #endregion

    public bool debugValues;

    void Update()
    {
        GetInput();
        ProcessAll();

        if (debugValues)
            DebugValues();
    }

    public void SetDebug(bool onOff)
    {
        debugValues = onOff;
    }
    private void DebugValues()
    {
        DebugPanel.Log("Back Button", backButton);
        DebugPanel.Log("Back Button Down", backButtonDown);
        DebugPanel.Log("Back Button Up", backButtonUp);

        DebugPanel.Log("A Button", aButton);
        DebugPanel.Log("A Button Down", aButtonDown);
        DebugPanel.Log("A Button Up", aButtonUp);

        DebugPanel.Log("B Button", bButton);
        DebugPanel.Log("B Button Down", bButtonDown);
        DebugPanel.Log("B Button Up", bButtonUp);

        DebugPanel.Log("X Button", xButton);
        DebugPanel.Log("X Button Down", xButtonDown);
        DebugPanel.Log("X Button Up", xButtonUp);

        DebugPanel.Log("Y Button", yButton);
        DebugPanel.Log("Y Button Down", yButtonDown);
        DebugPanel.Log("Y Button Up", yButtonUp);

        DebugPanel.Log("L3 Button", l3Button);
        DebugPanel.Log("L3 Button Down", l3ButtonDown);
        DebugPanel.Log("L3 Button Up", l3ButtonUp);

        DebugPanel.Log("R3 Button", r3Button);
        DebugPanel.Log("R3 Button Down", r3ButtonDown);
        DebugPanel.Log("R3 Button Up", r3ButtonUp);

        DebugPanel.Log("Left Trigger Value", leftTriggerValue);
        DebugPanel.Log("Left Trigger", leftTrigger);
        DebugPanel.Log("Left Trigger Down", leftTriggerDown);
        DebugPanel.Log("Left Trigger Up", leftTriggerUp);

        DebugPanel.Log("Right Trigger Value", rightTriggerValue);
        DebugPanel.Log("Right Trigger", rightTrigger);
        DebugPanel.Log("Right Trigger Down", rightTriggerDown);
        DebugPanel.Log("Right Trigger Up", rightTriggerUp);

        DebugPanel.Log("Left Grip Value", leftGripValue);
        DebugPanel.Log("Left Grip", leftGrip);
        DebugPanel.Log("Left Grip Down", leftGripDown);
        DebugPanel.Log("Left Grip Up", leftGripUp);

        DebugPanel.Log("Right Grip Value", rightGripValue);
        DebugPanel.Log("Right Grip ", rightGrip);
        DebugPanel.Log("Right Grip Down", rightGripDown);
        DebugPanel.Log("Right Grip Up", rightGripUp);
    }

    private void GetInput()
    {
        rightTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, OVRInput.Controller.All);
        rightGripValue = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger, OVRInput.Controller.All);

        leftTriggerValue = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, OVRInput.Controller.All);
        leftGripValue = OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger, OVRInput.Controller.All);

        leftStickValue = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, OVRInput.Controller.All);
        rightStickValue = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.All);

        backButton = OVRInput.Get(OVRInput.RawButton.Start, OVRInput.Controller.All);
        aButton = OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.All);
        bButton = OVRInput.Get(OVRInput.RawButton.B, OVRInput.Controller.All);
        xButton = OVRInput.Get(OVRInput.RawButton.X, OVRInput.Controller.All);
        yButton = OVRInput.Get(OVRInput.RawButton.Y, OVRInput.Controller.All);
        l3Button = OVRInput.Get(OVRInput.RawButton.LThumbstick, OVRInput.Controller.All);
        r3Button = OVRInput.Get(OVRInput.RawButton.RThumbstick, OVRInput.Controller.All);

        rightHandPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));
        rightHandRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        leftHandPosition = controllersParent.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch));
        leftHandRotation = controllersParent.rotation * OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
    }
    private void ProcessAll()
    {
        ProcessButton(nameof(backButton));
        ProcessButton(nameof(aButton));
        ProcessButton(nameof(bButton));
        ProcessButton(nameof(xButton));
        ProcessButton(nameof(yButton));
        ProcessButton(nameof(l3Button));
        ProcessButton(nameof(r3Button));

        ProcessAxis(nameof(leftTrigger));
        ProcessAxis(nameof(rightTrigger));
        ProcessAxis(nameof(leftGrip));
        ProcessAxis(nameof(rightGrip));
    }

    private void ProcessButton(string buttonName)
    {
        if ((bool)GetValue(buttonName) && !GetValueTriggered(buttonName) && !GetValueDown(buttonName))
        {
            SetValueDown(buttonName, true);
            SetValueTriggered(buttonName, true);
        }
        else if (GetValueDown(buttonName))
            SetValueDown(buttonName, false);
        if (!(bool)GetValue(buttonName) && GetValueTriggered(buttonName) && !GetValueUp(buttonName))
        {
            SetValueUp(buttonName, true);
            SetValueTriggered(buttonName, false);
        }
        else if (GetValueUp(buttonName))
            SetValueUp(buttonName, false);
    }
    private void ProcessAxis(string triggerName)
    {
        SetValue(triggerName, GetAxisFloat(triggerName) >= GetAxisThreshold(triggerName));
        ProcessButton(triggerName);
    }
    private void SetValue(string valueName, object value)
    {
        GetType().GetField(valueName).SetValue(this, value);
    }
    private object GetValue(string valueName)
    {
        return GetType().GetField(valueName).GetValue(this);
    }
    private void SetValueTriggered(string valueName, object value)
    {
        GetType().GetField(valueName + "Triggered").SetValue(this, value);
    }
    private bool GetValueTriggered(string valueName)
    {
        return (bool)GetType().GetField(valueName + "Triggered").GetValue(this);
    }
    private void SetValueDown(string valueName, object value)
    {
        GetType().GetField(valueName + "Down").SetValue(this, value);
    }
    private bool GetValueDown(string valueName)
    {
        return (bool)GetType().GetField(valueName + "Down").GetValue(this);
    }
    private void SetValueUp(string valueName, object value)
    {
        GetType().GetField(valueName + "Up").SetValue(this, value);
    }
    private bool GetValueUp(string valueName)
    {
        return (bool)GetType().GetField(valueName + "Up").GetValue(this);
    }
    private float GetAxisFloat(string axisName)
    {
        return (float)GetType().GetField(axisName + "Value").GetValue(this);
    }
    private float GetAxisThreshold(string axisName)
    {
        return (float)GetType().GetField(axisName + "Threshold").GetValue(this);
    }
}
