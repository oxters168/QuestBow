using UnityEngine;
using UnityHelpers;

public class TargetController : MonoBehaviour
{
    public TreeCollider treeCollider;
    [Tooltip("Should be in ascending order")]
    public ScoreColliderData[] targetScores;

    private void Start()
    {
        treeCollider.onCollided.AddListener(OnCollided);
    }

    private void OnCollided(TreeCollider.CollisionInfo colInfo)
    {
        if (!colInfo.isTrigger && colInfo.collidedWith.tag.Equals("Arrow"))
        {
            var arrow = colInfo.collidedWith.GetComponentInParent<ArrowController>();

            if (arrow != null && arrow.GetStuckTarget().root == transform)
            {
                arrow.scoreTarget = GetScore(arrow.GetTipPosition());

                if (arrow.scoreTarget != null)
                {
                    DebugPanel.Log("Arrow hit target", arrow.scoreTarget.score, 3);

                    var scoresPool = PoolManager.GetPool("ScoresPool");
                    scoresPool.Get<FlyingScoreController>(score =>
                    {
                        score.transform.position = transform.position;
                        score.transform.forward = -transform.forward;
                        score.scoreLabel.text = arrow.scoreTarget.score.ToString();
                        StartCoroutine(CommonRoutines.WaitToDoAction(success => { scoresPool.Return(score.transform); }, score.ttl));
                    });
                }
                else
                    DebugPanel.Log("Arrow hit target but somehow without a score", "", 3);
            }
        }
    }

    public ScoreColliderData GetScore(Vector3 position)
    {
        ScoreColliderData scoreColliderData = null;
        for (int i = 0; i < targetScores.Length; i++)
        {
            if (targetScores[i].col.transform.IsPointInside(position))
            {
                scoreColliderData = targetScores[i];
            }
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
