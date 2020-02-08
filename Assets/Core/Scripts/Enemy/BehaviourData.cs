using UnityEngine;

[System.Serializable]
public class Statetionary : RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<string, StateData> { }

[CreateAssetMenu(fileName = "BehaviourData", menuName = "Enemy/New Behaviour", order = 1)]
public class BehaviourData : ScriptableObject
{
    public Statetionary states;
}

[System.Serializable]
public class StateData
{
    public bool hasAnimationTrigger;
    public string animationTriggerName;
    public bool hasAnimationBool;
    public string animationBoolName;
    public bool hasAnimationFloat;
    public string animationFloatName;
    public float animationFloatValue;

    [Space(10), Tooltip("m/s")]
    public float speed = 0.33f;
    [Tooltip("deg/s")]
    public float rotationSpeed = 10f;
    public float minimumAngleOffset = 0.1f;
}
