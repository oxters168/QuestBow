using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Animator animator;

    [Tooltip("'Walk', 'Dead'")]
    public BehaviourData behaviour;
    
    [Tooltip("How long after death before they call expired action")]
    public float deathTime = 3;

    [Tooltip("The transform in the enemy that moves, tends to be the top transform")]
    public Transform selfTargetRoot;

    private StateData currentState;
    private bool isDead;
    private float diedAt;

    [Space(10)]
    public Transform enemyTarget;

    public virtual void Awake()
    {
        if (selfTargetRoot == null)
            selfTargetRoot = transform;
    }
    private void Update()
    {
        var state = GetCurrentState();
        ApplyState(state);

        if (!IsDead())
        {
            MoveStep(state);
        }
    }

    private void ApplyState(StateData state)
    {
        if (state != null && state != currentState)
        {
            if (currentState != null && currentState.hasAnimationBool)
                animator.SetBool(currentState.animationBoolName, false);
            if (state.hasAnimationBool)
                animator.SetBool(state.animationBoolName, true);

            if (state.hasAnimationTrigger)
                animator.SetTrigger(state.animationTriggerName);

            if (state.hasAnimationFloat)
                animator.SetFloat(state.animationFloatName, state.animationFloatValue);
        }
        currentState = state;
    }

    public virtual void Spawn(Vector3 position, Quaternion rotation, System.Action onExpired = null)
    {
        isDead = false;

        diedAt = -1;

        selfTargetRoot.position = position;
        selfTargetRoot.rotation = rotation;

        StartCoroutine(UnityHelpers.CommonRoutines.WaitToDoAction(success =>
        {
            ArrowController.DestroyArrowsInObject(transform);
            onExpired?.Invoke();
        }, 0, () =>
        {
            return IsDead() && Time.time - diedAt >= deathTime;
        }));
    }

    public virtual void MoveStep(StateData state)
    {
        selfTargetRoot.position += selfTargetRoot.forward * state.speed * Time.deltaTime;
        if (enemyTarget != null)
        {
            Vector3 targetDirection = (enemyTarget.position - selfTargetRoot.position).normalized;
            float targetAngle = Vector3.SignedAngle(selfTargetRoot.forward, targetDirection, Vector3.up);
            if (targetAngle < -state.minimumAngleOffset || targetAngle > state.minimumAngleOffset)
            {
                float nextRotation = Mathf.Min(Mathf.Abs(targetAngle), state.rotationSpeed * Time.deltaTime);
                selfTargetRoot.Rotate(Vector3.up * nextRotation * Mathf.Sign(targetAngle));
            }
        }
    }

    public virtual StateData GetCurrentState()
    {
        StateData currentState = null;

        if (IsDead() && behaviour.states.ContainsKey("Dead"))
            currentState = behaviour.states["Dead"];
        else if (!IsDead() && behaviour.states.ContainsKey("Walk"))
            currentState = behaviour.states["Walk"];

        return currentState;
    }

    public virtual void Kill()
    {
        diedAt = Time.time;
        isDead = true;
    }

    public virtual bool IsDead()
    {
        return isDead;
    }
}
