using UnityEngine;
using UnityHelpers;

public class BirdsGame : GenericGame
{
    private readonly GameVariables[] levelVariables = new GameVariables[]
    {
            new GameVariables() { name = "Practice", arrows = -1, countdownTime = 0, roundTime = -1 },
            new GameVariables() { name = "Time Attack", arrows = -1, countdownTime = 3, roundTime = 60 },
    };

    private int startArrowsFiredCount;
    public int totalScore { get; private set; }
    private float roundStartedAt;
    private int chosenLevel;
    private bool gameEnding;
    private float endGameWaitTime = 5;
    private Coroutine getReadyRoutine;

    public GameObject[] birdSpawns;
    private bool inGame;

    private void Update()
    {
        EndTimedGame();
    }

    private void EndTimedGame()
    {
        if (inGame && chosenLevel == 1 && (Time.time - roundStartedAt) > GetCountdownTime() + GetRoundTime() && !gameEnding)
            DoEndGameSequence();
    }

    public override void EndGame()
    {
        if (getReadyRoutine != null)
            StopCoroutine(getReadyRoutine);

        chosenLevel = -1;

        SetBirdsActive(false);
        inGame = false;
        SceneController.sceneControllerInScene.bowman.SetCanShoot(false);
    }
    public override void StartGame(int level)
    {
        roundStartedAt = Time.time;
        totalScore = 0;
        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;
        chosenLevel = level;

        SetBirdsActive(true);
        SceneController.sceneControllerInScene.bowman.SetCanShoot(true);

        //if (chosenLevel == 1)
            WaitForReady();
        //else
        //{
        //    inGame = true;
        //    roundStartedAt = Time.time;
        //}
    }
    private void WaitForReady()
    {
        getReadyRoutine = StartCoroutine(CommonRoutines.WaitToDoAction((isReady) =>
        {
            inGame = true;
            roundStartedAt = Time.time;
        }, 0, () => { return SceneController.sceneControllerInScene.bowman.bowHeld; }));
    }
    private void DoEndGameSequence()
    {
        gameEnding = true;
        SceneController.SetMenuAccess(false);
        SceneController.ShowGameModeMenu(false);
        SceneController.sceneControllerInScene.bowman.SetCanShoot(false);
        StartCoroutine(CommonRoutines.WaitToDoAction((success) =>
        {
            SceneController.EndGameStatic();
            SceneController.ShowGameModeMenu(true);
        }, endGameWaitTime));
    }

    public override bool IsPlaying()
    {
        return inGame;
    }
    public override int GetLevelArrowCount()
    {
        int arrowCount = 0;
        if (chosenLevel >= 0)
            arrowCount = levelVariables[chosenLevel].arrows;
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

    private void SetBirdsActive(bool onOff)
    {
        foreach (var birdSpawn in birdSpawns)
            birdSpawn.gameObject.SetActive(onOff);
    }

    public override float GetRoundStartTime()
    {
        return roundStartedAt;
    }

    public override float GetCountdownTime()
    {
        float countdownTime = 0;
        if (chosenLevel >= 0)
            countdownTime = levelVariables[chosenLevel].countdownTime;
        return countdownTime;
    }

    public override float GetRoundTime()
    {
        float roundTime = -1;
        if (chosenLevel >= 0)
            roundTime = levelVariables[chosenLevel].roundTime;
        return roundTime;
    }
}
