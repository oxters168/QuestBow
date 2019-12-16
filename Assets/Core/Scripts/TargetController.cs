using UnityEngine;
using UnityHelpers;

public class TargetController : MonoBehaviour
{
    public TreeCollider treeCollider;
    [Tooltip("Should be in ascending order")]
    public ScoreColliderData[] targetScores;

    public event ScoreHandler onArrowHit;
    public delegate void ScoreHandler(TargetController caller, ArrowController arrow);

    private void Start()
    {
        treeCollider.onCollided.AddListener(OnCollided);
    }

    private void OnCollided(TreeCollider.CollisionInfo colInfo)
    {
        if (!colInfo.isTrigger && colInfo.collidedWith.tag.Equals("Arrow"))
        {
            var arrow = colInfo.collidedWith.GetComponentInParent<ArrowController>();

            if (arrow != null)
            {
                var contactPoint = colInfo.collision.GetContact(0);
                var scoreTarget = GetScore(contactPoint.point);
                if (scoreTarget != null)
                    arrow.scoreTarget = scoreTarget;
                //arrow.scoreTarget = GetScore(arrow.GetTipPosition());

                if (arrow.scoreTarget != null && arrow.GetStuckTarget().GetComponentInParent<TargetController>() == this)
                {
                    DebugPanel.Log("Arrow hit target", arrow.scoreTarget.score, 3);

                    onArrowHit?.Invoke(this, arrow);

                    var scoresPool = PoolManager.GetPool("ScoresPool");
                    scoresPool.Get<FlyingScoreController>(score =>
                    {
                        score.transform.position = transform.position;
                        score.transform.forward = -transform.forward;
                        score.scoreLabel.text = arrow.scoreTarget.score.ToString();
                        StartCoroutine(CommonRoutines.WaitToDoAction(s => { scoresPool.Return(score.transform); }, score.ttl));
                    });
                }
                else
                    DebugPanel.Log("Arrow hit target but somehow without a score", "", 3);
            }
        }
    }

    private void CheckPunctured(ArrowController arrow)
    {
        StartCoroutine(CommonRoutines.WaitToDoAction((success) =>
        {
        }, 3, () => { return arrow.GetStuckTarget() != null && arrow.GetStuckTarget() == transform; }));
    }

    public ScoreColliderData GetScore(Vector3 position)
    {
        ScoreColliderData scoreColliderData = null;
        for (int i = 0; i < targetScores.Length; i++)
        {
            if (targetScores[i].col.transform.IsPointInside(position))
                scoreColliderData = targetScores[i];
        }
        return scoreColliderData;
    }

    [System.Serializable]
    public class ScoreColliderData
    {
        public Collider col;
        public int score;
    }
}
