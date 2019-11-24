using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Tooltip("m/s")]
    public float speed = 1;
    [Tooltip("deg/s")]
    public float rotationSpeed = 1f;
    public float minimumAngleOffset = 0.1f;

    public Transform target;
    public RootMotion.Dynamics.PuppetMaster puppet;
    public Transform accurateSelf;

    private void Update()
    {
        if (puppet.isAlive)
        {
            transform.position += accurateSelf.forward * speed * Time.deltaTime;
            if (target != null)
            {
                Vector3 targetDirection = (target.position - transform.position).normalized;
                float targetAngle = Vector3.SignedAngle(accurateSelf.forward, targetDirection, Vector3.up);
                if (targetAngle < -minimumAngleOffset || targetAngle > minimumAngleOffset)
                {
                    float nextRotation = Mathf.Min(Mathf.Abs(targetAngle), rotationSpeed * Time.deltaTime);
                    transform.Rotate(Vector3.up * nextRotation * Mathf.Sign(targetAngle));
                }
            }
        }
    }
}
