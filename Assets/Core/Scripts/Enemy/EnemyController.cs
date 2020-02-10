using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    public Animator animator;

    [Tooltip("'Walk', 'Dead'")]
    public BehaviourData behaviour;

    private float currentAttackLength, lastAttackTime;

    [Tooltip("The transform in the enemy that moves, tends to be the top transform")]
    public Transform selfTargetRoot;

    private StateData previousState;
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
        if (state != null && state != previousState)
        {
            if (previousState != null && previousState.hasAnimationBool)
                animator.SetBool(previousState.animationBoolName, false);
            if (state.hasAnimationBool)
                animator.SetBool(state.animationBoolName, true);

            if (state.hasAnimationTrigger)
                animator.SetTrigger(state.animationTriggerName);

            if (state.hasAnimationFloat)
                animator.SetFloat(state.animationFloatName, state.animationFloatValue);

            if (state.isAttack)
            {
                var enemyHealth = enemyTarget?.GetComponentInChildren<HealthController>();
                if (enemyHealth != null)
                    enemyHealth.HurtValue(Random.Range(state.minAttackDamage, state.maxAttackDamage));
            }
        }
        previousState = state;
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
            return IsDead() && Time.time - diedAt >= behaviour.deathTime;
        }));
    }

    public virtual void MoveStep(StateData state)
    {
        if (state != null)
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
    }

    public virtual StateData GetCurrentState()
    {
        StateData currentState = null;

        if (currentAttackLength <= 0 || Time.time - lastAttackTime > currentAttackLength)
        {
            currentAttackLength = 0;

            if (behaviour.states.ContainsKey("Idle"))
                currentState = behaviour.states["Idle"];

            if (IsDead() && behaviour.states.ContainsKey("Dead"))
                currentState = behaviour.states["Dead"];
            else if (!IsDead())
            {
                if (enemyTarget != null && (selfTargetRoot.position - enemyTarget.position).sqrMagnitude < behaviour.attackDistance * behaviour.attackDistance)
                {
                    var attacks = behaviour.states.Values.Where(state => state.isAttack).ToArray();
                    if (attacks.Length > 0)
                    {
                        int attackIndex = Random.Range(0, attacks.Length - 1);
                        currentState = attacks[attackIndex];
                        currentAttackLength = Random.Range(currentState.minAttackLength, currentState.maxAttackLength);
                        lastAttackTime = Time.time;
                    }
                }
                else if (behaviour.states.ContainsKey("Walk"))
                    currentState = behaviour.states["Walk"];
            }
        }

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
