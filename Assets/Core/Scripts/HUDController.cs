using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public int arrowsLeft;
    public TextMeshProUGUI arrowsLeftLabel;
    public int score;
    public TextMeshProUGUI scoreLabel;
    public bool showGetReady;
    public TextMeshProUGUI getReadyLabel;
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

        getReadyLabel.gameObject.SetActive(showGetReady);

        countdownLabel.gameObject.SetActive(!showGetReady && startCountdown > 0);
        countdownLabel.text = Mathf.CeilToInt(startCountdown).ToString();

        timeLeftTitleLabel.gameObject.SetActive(!showGetReady && roundTimeLeft >= 0);
        roundTimeLeftLabel.gameObject.SetActive(!showGetReady && roundTimeLeft >= 0);
        roundTimeLeftLabel.text = UnityHelpers.MathHelpers.SetDecimalPlaces(roundTimeLeft, 1).ToString();
    }
}
