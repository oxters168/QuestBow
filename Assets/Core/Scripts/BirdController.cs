using UnityEngine;
using UnityHelpers;

public class BirdController : MonoBehaviour
{
    public GameObject[] birdSkins;
    public Animator birdAnimator;
    public Rigidbody birdBody;
    [Tooltip("m/s")]
    public float speed = 5;
    public float glideMin = 3, glideMax = 5, flapMin = 2, flapMax = 4;
    public bool isGliding;
    public bool isDead;
    private float glideTime, glideStart, flapTime, flapStart;

    private void Start()
    {
        SetRandomSkin();
        SetAnimTime();
    }
    private void Update()
    {
        RefreshAnimator();
        Move();
    }

    public void Kill()
    {
        isDead = true;
        birdBody.isKinematic = false;
    }
    public void Revive()
    {
        isDead = false;
        birdBody.isKinematic = true;
    }
    public void SetRandomSkin()
    {
        SetSkin(Random.Range(0, birdSkins.Length));
    }
    public void SetSkin(int index)
    {
        index = Mathf.Clamp(index, 0, birdSkins.Length);
        for (int i = 0; i < birdSkins.Length; i++)
            birdSkins[i].SetActive(i == index);
    }
    private void SetAnimTime()
    {
        if (isGliding)
        {
            glideStart = Time.time;
            glideTime = Random.Range(glideMin, glideMax);
        }
        else
        {
            flapStart = Time.time;
            flapTime = Random.Range(flapMin, flapMax);
        }
    }
    private void RefreshAnimator()
    {
        if (!isDead)
        {
            if (isGliding && Time.time - glideStart > glideTime)
            {
                isGliding = false;
                SetAnimTime();
            }
            else if (!isGliding && Time.time - flapStart > flapTime)
            {
                isGliding = true;
                SetAnimTime();
            }
        }
        birdAnimator.SetBool("Flap", !isGliding);
        birdAnimator.SetBool("Dead", isDead);
        birdAnimator.SetBool("Grounded", transform.IsGrounded(0.1f, true));
    }
    private void Move()
    {
        if (!isDead)
            transform.position += transform.forward * speed * Time.deltaTime;
    }
}
