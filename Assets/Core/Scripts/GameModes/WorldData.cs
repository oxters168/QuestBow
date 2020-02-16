using System;
using UnityEngine;

public class WorldData : MonoBehaviour
{
    public enum GameType { none, targets, birds, horde, }

    public GameType currentGameMode { get; private set; }
    public int difficulty { get; private set; }

    public GenericGame[] gameModes;

    public void SetGameMode(GameType gameMode, int difficulty = 0)
    {
        currentGameMode = gameMode;

        for (int i = 0; i < gameModes.Length; i++)
            if (i + 1 == (int)currentGameMode)
                gameModes[i].StartGame(difficulty);
            else
                gameModes[i].EndGame();
    }

    public int GetArrowsLeft()
    {
        int arrowsLeft = 0;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            arrowsLeft = gameMode.GetArrowsLeft();

        return arrowsLeft;
    }
    public int GetScore()
    {
        int score = 0;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            score = gameMode.GetScore();

        return score;
    }
    public float GetRoundTimeStarted()
    {
        float roundTimeStarted = 0;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            roundTimeStarted = gameMode.GetRoundStartTime();
        return roundTimeStarted;
    }
    public float GetCountdownTime()
    {
        float countdownTime = 0;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            countdownTime = gameMode.GetCountdownTime();
        return countdownTime;
    }
    public float GetRoundTime()
    {
        float roundTime = -1;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            roundTime = gameMode.GetRoundTime();
        return roundTime;
    }
    public bool GetIsPlaying()
    {
        bool inGame = false;
        var gameMode = GetCurrentGameMode();
        if (gameMode != null)
            inGame = gameMode.IsPlaying();
        return inGame;
    }
    public GenericGame GetCurrentGameMode()
    {
        GenericGame gameMode = null;

        if (currentGameMode != GameType.none)
            gameMode = gameModes[((int)currentGameMode) - 1];

        return gameMode;
    }
    public void SetTargetGameRandomInterval(float seconds)
    {
        DoActionToTargetGames((targetGame) => targetGame.SetRandomTargetInterval(seconds));
    }
    public void SetTargetGamePracticeDistance(float meters)
    {
        DoActionToTargetGames((targetGame) => targetGame.SetPracticeDistance(meters));
    }
    public void SetTargetGamePracticeHeight(float meters)
    {
        DoActionToTargetGames((targetGame) => targetGame.SetPracticeHeight(meters));
    }
    public void DoActionToTargetGames(Action<TargetGame> action)
    {
        foreach (var gameMode in gameModes)
            if (gameMode is TargetGame)
                action?.Invoke((TargetGame)gameMode);
    }
}
