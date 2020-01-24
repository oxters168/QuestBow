using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public int arrowsLeft;
    public TextMeshProUGUI arrowsLeftLabel;
    public int score;
    public TextMeshProUGUI scoreLabel;
    public float startCountdown;
    public TextMeshProUGUI countdownLabel;
    public float roundTimeLeft;
    public TextMeshProUGUI roundTimeLeftLabel;
    public TextMeshProUGUI timeLeftTitleLabel;

    public GameObject infiniteArrowsLeftImage;

    void Update()
    {
        bool hasFiniteArrows = arrowsLeft < int.MaxValue;
        arrowsLeftLabel.gameObject.SetActive(hasFiniteArrows);
        infiniteArrowsLeftImage.SetActive(!hasFiniteArrows);

        arrowsLeftLabel.text = arrowsLeft.ToString();
        scoreLabel.text = score.ToString();

        countdownLabel.gameObject.SetActive(startCountdown > 0);
        countdownLabel.text = Mathf.CeilToInt(startCountdown).ToString();

        timeLeftTitleLabel.gameObject.SetActive(roundTimeLeft >= 0);
        roundTimeLeftLabel.gameObject.SetActive(roundTimeLeft >= 0);
        roundTimeLeftLabel.text = UnityHelpers.MathHelpers.SetDecimalPlaces(roundTimeLeft, 1).ToString();
    }
}
