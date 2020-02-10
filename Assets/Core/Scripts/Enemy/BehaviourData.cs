using UnityEngine;

[System.Serializable]
public class Statetionary : RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<string, StateData> { }

[CreateAssetMenu(fileName = "BehaviourData", menuName = "Enemy/New Behaviour", order = 1)]
public class BehaviourData : ScriptableObject
{
    [Tooltip("The minimum distance from the target to be able to attack it")]
    public float attackDistance = 3;
    [Tooltip("How long after death before they call expired action")]
    public float deathTime = 3;

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
    public bool isAttack;
    [Tooltip("How many seconds before they can do something else")]
    public float minAttackLength, maxAttackLength;
    public float minAttackDamage, maxAttackDamage;
}
