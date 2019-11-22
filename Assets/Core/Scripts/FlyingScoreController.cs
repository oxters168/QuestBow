using UnityEngine;

public class FlyingScoreController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI scoreLabel;
    public Vector3 direction = Vector3.up;
    [Tooltip("m/s")]
    public float speed = 0.2f;
    public float lerp = 5;
    [Range(0, float.MaxValue)]
    public float ttl = 5;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
