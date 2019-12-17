using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public int arrowsLeft;
    public TextMeshProUGUI arrowsLeftLabel;
    public int score;
    public TextMeshProUGUI scoreLabel;

    void Update()
    {
        arrowsLeftLabel.text = arrowsLeft < int.MaxValue ? arrowsLeft.ToString() : "∞";
        scoreLabel.text = score.ToString();
    }
}
