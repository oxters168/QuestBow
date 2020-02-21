using UnityEngine;
using UnityHelpers;

public class TargetController : MonoBehaviour
{
    public TreeCollider treeCollider;
    [Tooltip("Should be in ascending order")]
    public ScoreColliderData[] targetScores;

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
