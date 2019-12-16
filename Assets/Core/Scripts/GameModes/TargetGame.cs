using UnityEngine;
using UnityHelpers;

public class TargetGame : GenericGame
{
    public TargetController practiceTarget;
    private bool inGame;
    public int totalScore { get; private set; }
    private int startArrowsFiredCount;

    private void Start()
    {
        practiceTarget.onArrowHit += Target_onArrowHit;
    }

    private void Target_onArrowHit(TargetController caller, ArrowController arrow)
    {
        totalScore += arrow.scoreTarget.score;
    }

    private void SetTargetsActive(bool onOff)
    {
        practiceTarget.gameObject.SetActive(onOff);
    }

    public void SetPracticeDistance(float meters)
    {
        float height = practiceTarget.transform.position.y;
        practiceTarget.transform.position = SceneController.sceneControllerInScene.bowman.transform.position - practiceTarget.transform.forward * meters + Vector3.up * height;
    }
    public void SetPracticeHeight(float meters)
    {
        practiceTarget.transform.position = new Vector3(practiceTarget.transform.position.x, meters, practiceTarget.transform.position.z);
    }

    public override void EndGame()
    {
        SceneController.sceneControllerInScene.bowman.onArrowShot -= Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        SetTargetsActive(false);
        inGame = false;
    }

    public override void StartGame(int level)
    {
        totalScore = 0;
        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;
        SceneController.sceneControllerInScene.bowman.onArrowShot += Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        SetTargetsActive(true);
        inGame = true;

        SceneController.sceneControllerInScene.bowman.canShoot = true;
    }

    public override int GetArrowsLeft()
    {
        return 5 - (SceneController.sceneControllerInScene.bowman.totalArrowsFired - startArrowsFiredCount);
    }
    public override int GetScore()
    {
        return totalScore;
    }

    private void Bowman_onArrowShot()
    {
        if (GetArrowsLeft() <= 0)
            SceneController.sceneControllerInScene.bowman.canShoot = false;
    }

    public override bool IsPlaying()
    {
        return inGame;
    }
}
