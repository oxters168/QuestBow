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
                gameModes[i].StartGame();
            else
                gameModes[i].EndGame();
    }
}
