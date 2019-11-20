using System.Collections;
using UnityEngine;
using UnityHelpers;

public class ArrowController : MonoBehaviour
{
    public Rigidbody mainBody, tipBody;
    private FixedJoint joint;
    public TreeCollider treeCollider;

    private void Start()
    {
        joint = GetComponent<FixedJoint>();
        treeCollider.onCollided.AddListener(OnCollided);
    }

    private void OnCollided(TreeCollider.CollisionInfo colInfo)
    {
        if (!colInfo.isTrigger && !colInfo.collidedWith.tag.Equals("Arrow"))
        {
            SetStuck(true, colInfo.collidedWith.transform);
        }
    }
    private void SetStuck(bool onOff, Transform parent = null)
    {
        mainBody.isKinematic = onOff;
        tipBody.isKinematic = onOff;
        transform.SetParent(parent);
        if (parent == null)
            transform.localScale = Vector3.one;
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
    /*public IEnumerator Translate(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        return ResetPhysicsProperly(() =>
        {
            transform.position = position;
            transform.rotation = rotation;
        },
        () =>
        {
            mainBody.velocity = velocity;
        });
    }
    private IEnumerator ResetPhysicsProperly(System.Action preWait = null, System.Action onReset = null)
    {
        SetStuck(false);

        preWait?.Invoke();

        ResetPhysics();
        mainBody.Sleep();
        tipBody.Sleep();

        yield return new WaitForFixedUpdate();

        DestroyImmediate(joint);
        joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = tipBody;

        ResetPhysics();

        yield return new WaitForFixedUpdate();

        mainBody.WakeUp();
        tipBody.WakeUp();

        ResetPhysics();

        yield return new WaitForFixedUpdate();

        ResetPhysics();

        onReset?.Invoke();
    }*/
}
