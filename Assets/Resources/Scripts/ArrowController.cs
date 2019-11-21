using UnityEngine;
using UnityHelpers;

public class ArrowController : MonoBehaviour
{
    public Rigidbody mainBody, tipBody;
    private FixedJoint joint;
    public TreeCollider treeCollider;
    //public float minimumPenetrationSpeed = 2;
    public float minimumPenetrationAngle = 30;

    public TargetController.ScoreColliderData scoreTarget { get; internal set; }

    public Vector3 previousVelocity { get; private set; }
    public Vector3 previousForward { get; private set; }

    private void Start()
    {
        joint = GetComponent<FixedJoint>();
        treeCollider.onCollided.AddListener(OnCollided);
    }
    private void FixedUpdate()
    {
        if (tipBody != null)
        {
            previousVelocity = mainBody.velocity;
            previousForward = mainBody.transform.forward;
        }
    }

    public Transform GetStuckTarget()
    {
        return transform.parent;
    }
    private void OnCollided(TreeCollider.CollisionInfo colInfo)
    {
        if (!colInfo.isTrigger && colInfo.collisionState == TreeCollider.CollisionInfo.CollisionState.enter && !colInfo.collidedWith.tag.Equals("Arrow"))
        {
            float penetrationAngle = Vector3.Angle(previousForward, previousVelocity.normalized);
            DebugPanel.Log(name + " puncture angle", penetrationAngle, 5);
            if (penetrationAngle <= minimumPenetrationAngle)
            {
                var contactPoint = colInfo.collision.GetContact(0);
                Vector3 punctureDirection = previousForward;
                Debug.DrawRay(contactPoint.point, -punctureDirection, Color.red, 1000);
                float arrowLength = transform.GetTotalBounds(false, false).size.z;
                Vector3 stuckPosition = contactPoint.point - punctureDirection * arrowLength;
                SetStuck(stuckPosition, punctureDirection, colInfo.collidedWith.transform);
            }
        }
    }
    public Vector3 GetTipPosition()
    {
        float arrowLength = transform.GetTotalBounds(false, false).size.z;
        return transform.position + transform.forward * arrowLength;
    }
    private void SetStuck(Vector3 position, Vector3 forward, Transform parent)
    {
        Destroy(joint);
        Destroy(mainBody);
        Destroy(tipBody);
        //mainBody.isKinematic = onOff;
        //tipBody.isKinematic = onOff;
        transform.position = position;
        transform.forward = forward;
        transform.SetParent(parent);
        //if (parent == null)
        //    transform.localScale = Vector3.one;
    }

    public void Translate(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        transform.position = position;
        transform.rotation = rotation;

        ResetPhysics();

        mainBody.velocity = velocity;
    }
    public void ResetPhysics()
    {
        mainBody.velocity = Vector3.zero;
        mainBody.angularVelocity = Vector3.zero;
        tipBody.velocity = Vector3.zero;
        tipBody.angularVelocity = Vector3.zero;
    }
}
