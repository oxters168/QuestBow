using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public Rigidbody mainBody, tipBody;

    public void ResetPhysics()
    {
        mainBody.velocity = Vector3.zero;
        mainBody.angularVelocity = Vector3.zero;
        tipBody.velocity = Vector3.zero;
        tipBody.angularVelocity = Vector3.zero;
    }
}
