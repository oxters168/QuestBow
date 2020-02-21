using TMPro;
using UnityEngine;

public class StatsViewController : MonoBehaviour
{
    public static StatsViewController statsViewInScene { get; private set; }

    public StatListItem scoreLabel, killsLabel, arrowsShotLabel, arrowsHitLabel, accuracyLabel, wavesDefeatedLabel, damageDealtLabel, damageReceivedLabel, amountHealedLabel;
    private static int arrowsShot, arrowsHit;

    private void Awake()
    {
        statsViewInScene = this;
    }
    private void OnDisable()
    {
        arrowsShot = 0;
        arrowsHit = 0;

        DisableStatItems();
    }

    private void DisableStatItems()
    {
        scoreLabel.gameObject.SetActive(false);
        killsLabel.gameObject.SetActive(false);
        arrowsShotLabel.gameObject.SetActive(false);
        arrowsHitLabel.gameObject.SetActive(false);
        accuracyLabel.gameObject.SetActive(false);
        wavesDefeatedLabel.gameObject.SetActive(false);
        damageDealtLabel.gameObject.SetActive(false);
        damageReceivedLabel.gameObject.SetActive(false);
        amountHealedLabel.gameObject.SetActive(false);
    }

    public static void SetScore(int value)
    {
        SetValueOfLabel(statsViewInScene.scoreLabel, value);
    }
    public static void SetKills(int value)
    {
        SetValueOfLabel(statsViewInScene.killsLabel, value);
    }
    public static void SetArrowsShot(int value)
    {
        arrowsShot = value;
        UpdateAccuracy();
        SetValueOfLabel(statsViewInScene.arrowsShotLabel, value);
    }
    public static void SetArrowsHit(int value)
    {
        arrowsHit = value;
        UpdateAccuracy();
        SetValueOfLabel(statsViewInScene.arrowsHitLabel, value);
    }
    private static void UpdateAccuracy()
    {
        int percent = Mathf.RoundToInt(arrowsHit / (arrowsShot > 0 ? (float)arrowsShot : 1) * 100);
        //SetValueOfLabel(statsViewInScene.accuracyLabel, Mathf.RoundToInt(arrowsHit / (arrowsShot > 0 ? (float)arrowsShot : 1) * 100));
        statsViewInScene.accuracyLabel.gameObject.SetActive(true);
        statsViewInScene.accuracyLabel.valueLabel.text = percent + "%";
    }
    public static void SetWavesDefeated(int value)
    {
        SetValueOfLabel(statsViewInScene.wavesDefeatedLabel, value);
    }
    public static void SetDamageDealt(int value)
    {
        SetValueOfLabel(statsViewInScene.damageDealtLabel, value);
    }
    public static void SetDamageReceived(int value)
    {
        SetValueOfLabel(statsViewInScene.damageReceivedLabel, value);
    }
    public static void SetAmountHealed(int value)
    {
        SetValueOfLabel(statsViewInScene.amountHealedLabel, value);
    }

    private static void SetValueOfLabel(StatListItem listItem, int value)
    {
        listItem.gameObject.SetActive(true);
        listItem.valueLabel.text = value.ToString();
    }
}
