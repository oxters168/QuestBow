using UnityEngine;
using UnityHelpers;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class AudioTagtionary : SerializableDictionaryBase<string, AudioClip> { }

public class ArrowController : MonoBehaviour
{
    [Tooltip("The key is the tag for hit gameobjects. If the tag can't be found will use first clip.")]
    public AudioTagtionary punctureClips;
    [Tooltip("Same as punctureClips except these clips are for one just hitting without penetration.")]
    public AudioTagtionary hitClips;

    public AudioSource flightAudio;
    public AudioSource hitAudio;

    public Transform renderingRoot;
    public Rigidbody mainBody;
    public TreeCollider treeCollider;
    public float minimumPenetrationSpeedPercent = 0.2f;
    public float minimumPenetrationAngle = 30;
    public float maxHurtValue = 20;

    public TargetController.ScoreColliderData scoreTarget { get; internal set; }

    public Vector3 previousVelocity { get; private set; }
    public Vector3 previousForward { get; private set; }

    public Vector3 shotPosition { get; private set; }
    public Quaternion shotRotation { get; private set; }
    public Vector3 shotVelocity { get; private set; }
    public float shotSpeedSqr;
    public float speedPercent { get; private set; }

    private void Start()
    {
        treeCollider.onCollided.AddListener(OnCollided);
    }
    private void Update()
    {
        if (shotSpeedSqr > 0)
            speedPercent = previousVelocity.sqrMagnitude / shotSpeedSqr;
        else
            speedPercent = 0;

        RefreshFlightAudio();
    }
    private void FixedUpdate()
    {
        if (mainBody)
        {
            previousVelocity = mainBody.velocity;
            previousForward = mainBody.transform.forward;
        }
    }

    private void RefreshFlightAudio()
    {
        if (speedPercent > 0)
        {
            speedPercent = previousVelocity.sqrMagnitude / shotSpeedSqr;
            flightAudio.volume = speedPercent;
            if (!flightAudio.isPlaying)
                flightAudio.Play();
        }
        else if (flightAudio.isPlaying)
            flightAudio.Stop();
    }
    private void PlayPunctureAudio(string tag)
    {
        AudioClip punctureClip = null;
        if (punctureClips != null)
        {
            if (!string.IsNullOrEmpty(tag) && punctureClips.ContainsKey(tag))
                punctureClip = punctureClips[tag];
            else
                punctureClip = punctureClips.First().Value;
        }
        else
            Debug.LogError("No hit audio clips for arrow found");

        if (punctureClip != null)
        {
            hitAudio.clip = punctureClip;
            hitAudio.Play();
        }
    }
    private void PlayHitAudio(string tag)
    {
        AudioClip hitClip = null;
        if (hitClips != null)
        {
            if (!string.IsNullOrEmpty(tag) && hitClips.ContainsKey(tag))
                hitClip = hitClips[tag];
            else
                hitClip = hitClips.First().Value;
        }
        else
            Debug.LogError("No hit audio clips for arrow found");

        if (hitClip != null)
        {
            hitAudio.clip = hitClip;
            hitAudio.Play();
        }
    }

    public Transform GetStuckTarget()
    {
        return transform.parent;
    }
    private void OnCollided(TreeCollider.CollisionInfo colInfo)
    {
        if (!colInfo.isTrigger && colInfo.collisionState == TreeCollider.CollisionInfo.CollisionState.enter)
        {
            #region Health
            HealthController hitHealth = colInfo.collidedWith.GetComponentInParent<HealthController>();
            if (hitHealth != null)
            {
                //Need to calculate hurt value
                hitHealth.HurtValue(maxHurtValue);
            }
            #endregion
            #region Physics
            Rigidbody hitBody = colInfo.collidedWith.GetComponentInParent<Rigidbody>();
            if (hitBody != null)
                hitBody.AddForce(previousVelocity, ForceMode.Impulse);
            #endregion
            #region Score
            ArrowController otherArrow = colInfo.collidedWith.GetComponentInParent<ArrowController>();
            if (otherArrow != null)
            {
                scoreTarget = otherArrow.scoreTarget;
            }
            #endregion
            #region Penetration
            float penetrationAngle = Vector3.Angle(previousForward, previousVelocity.normalized);
            DebugPanel.Log(name + " puncture angle", penetrationAngle, 5);
            DebugPanel.Log(name + " speed percent", speedPercent);
            if (speedPercent > minimumPenetrationSpeedPercent && penetrationAngle <= minimumPenetrationAngle)
            {
                PlayPunctureAudio(colInfo.collidedWith.tag);

                var contactPoint = colInfo.collision.GetContact(0);
                Vector3 punctureDirection = previousForward;
                Debug.DrawRay(contactPoint.point, -punctureDirection, Color.red, 1000);
                SetStuck(contactPoint.point, punctureDirection, colInfo.collidedWith.transform);
            }
            else
                PlayHitAudio(colInfo.collidedWith.tag);
            #endregion
        }
    }
    public Vector3 GetTipPosition()
    {
        float arrowLength = renderingRoot.GetTotalBounds(LayerMask.GetMask("Arrow")).size.z;
        return transform.position + transform.forward * arrowLength;
    }
    private void SetStuck(Vector3 position, Vector3 forward, Transform parent)
    {
        if (mainBody)
            Destroy(mainBody);
        else
            Debug.LogWarning("ArrowController: Tried to destroy mainBody, but it doesn't exist");
        //mainBody.isKinematic = onOff;
        //tipBody.isKinematic = onOff;

        transform.SetParent(parent);

        float arrowLength = renderingRoot.GetTotalBounds(false, false).size.z;
        transform.position = position - forward * arrowLength;
        transform.forward = forward;
        //if (parent == null)
        //    transform.localScale = Vector3.one;
    }

    public static void DestroyArrowsInObject(Transform root)
    {
        var arrowsInObject = root.GetComponentsInChildren<ArrowController>();
        for (int i = 0; i < arrowsInObject.Length; i++)
            Destroy(arrowsInObject[i].gameObject);
    }

    public void Shoot(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        shotPosition = position;
        shotRotation = rotation;
        shotVelocity = velocity;
        //shotSpeedSqr = shotVelocity.sqrMagnitude;

        transform.position = shotPosition;
        transform.rotation = shotRotation;

        ResetPhysics();

        mainBody.velocity = shotVelocity;
    }
    public void ResetPhysics()
    {
        mainBody.velocity = Vector3.zero;
        mainBody.angularVelocity = Vector3.zero;
    }
}
