using UnityEngine;

public class TargetGame : GenericGame
{
    private readonly int[] availableArrows = new int[] { -1, 6 };
    public TargetController practiceTarget;
    public TargetController olympicTarget;
    private int chosenLevel;
    private bool inGame;
    public int totalScore { get; private set; }
    private int startArrowsFiredCount;

    private void Start()
    {
        practiceTarget.onArrowHit += Target_onArrowHit;
        olympicTarget.onArrowHit += Target_onArrowHit;
    }

    private void Target_onArrowHit(TargetController caller, ArrowController arrow)
    {
        totalScore += arrow.scoreTarget.score;
    }

    private void SetTargetsActive(bool onOff)
    {
        practiceTarget.gameObject.SetActive(false);
        olympicTarget.gameObject.SetActive(false);

        if (onOff)
        {
            if (chosenLevel == 0)
                practiceTarget.gameObject.SetActive(true);
            else if (chosenLevel == 1)
                olympicTarget.gameObject.SetActive(true);
        }
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

    public override void StartGame(int level)
    {
        totalScore = 0;
        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;
        SceneController.sceneControllerInScene.bowman.onArrowShot += Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        chosenLevel = level;
        SetTargetsActive(true);
        inGame = true;

        SceneController.sceneControllerInScene.bowman.canShoot = true;
    }
    public override void EndGame()
    {
        chosenLevel = -1;

        SceneController.sceneControllerInScene.bowman.onArrowShot -= Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        SetTargetsActive(false);
        inGame = false;
    }
    public override int GetLevelArrowCount()
    {
        int arrowCount = 0;
        if (chosenLevel >= 0)
            arrowCount = availableArrows[chosenLevel];
        return arrowCount;
    }
    public override int GetArrowsLeft()
    {
        int arrowsLeft = GetLevelArrowCount();
        if (arrowsLeft >= 0)
            arrowsLeft -= SceneController.sceneControllerInScene.bowman.totalArrowsFired - startArrowsFiredCount;
        else
            arrowsLeft = int.MaxValue;

        return arrowsLeft;
    }
    public override int GetScore()
    {
        return totalScore;
    }

    private void Bowman_onArrowShot()
    {
        if (inGame && GetArrowsLeft() <= 0)
            SceneController.sceneControllerInScene.bowman.canShoot = false;
    }

    public override bool IsPlaying()
    {
        return inGame;
    }
}
