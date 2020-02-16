using UnityEngine;
using UnityHelpers;

public class HordeGame : GenericGame
{
    private readonly GameVariables[] levelVariables = new GameVariables[]
    {
        new GameVariables() { name = "Practice", arrows = -1, countdownTime = 0, roundTime = -1 },
        new GameVariables() { name = "Waves", arrows = -1, countdownTime = 3, roundTime = -1 },
    };

    private int startArrowsFiredCount;
    public int totalScore { get; private set; }
    private float roundStartedAt;
    private int chosenLevel;

    public PoolSpawner[] enemySpawns;
    private bool inGame;

    public override void EndGame()
    {
        SetEnemiesActive(false);
        inGame = false;
    }
    public override void StartGame(int level)
    {
        roundStartedAt = Time.time;
        totalScore = 0;
        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;

        inGame = true;

        chosenLevel = level;

        SetEnemiesActive(true);

        SceneController.sceneControllerInScene.bowman.canShoot = true;
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

    private void SetEnemiesActive(bool onOff)
    {
        foreach (var enemySpawn in enemySpawns)
        {
            enemySpawn.gameObject.SetActive(onOff);
            if (!onOff)
                enemySpawn.ReturnAll();
        }
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
