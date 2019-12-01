using UnityEngine;

public class WorldData : MonoBehaviour
{
    public enum GameType { none, targets, birds, horde, }

    public GameType currentGameMode { get; private set; }
    public int difficulty { get; private set; }

    public TargetController[] targets;
    public GameObject[] birdSpawns;
    public GameObject[] enemySpawns;

    public void SetGameMode(GameType gameMode, int difficulty = 0)
    {
        currentGameMode = gameMode;
        if (gameMode != GameType.targets)
            HideTargets();

        if (gameMode != GameType.birds)
            HideBirds();

        if (gameMode != GameType.horde)
            HideEnemies();

        SetDifficulty(difficulty);
    }

    public void SetDifficulty(int value)
    {
        difficulty = Mathf.Clamp(value, 0, int.MaxValue);
        if (currentGameMode == GameType.targets)
            foreach (var target in targets)
                target.gameObject.SetActive(true);

        if (currentGameMode == GameType.birds)
        {
            for (int i = 0; i < birdSpawns.Length; i++)
                birdSpawns[i].gameObject.SetActive(i <= difficulty);
        }

        if (currentGameMode == GameType.horde)
        {
            for (int i = 0; i < enemySpawns.Length; i++)
                enemySpawns[i].gameObject.SetActive(i <= difficulty);
        }
    }

    private void HideTargets()
    {
        foreach (var target in targets)
            target.gameObject.SetActive(false);
    }
    private void HideBirds()
    {
        foreach (var birdSpawn in birdSpawns)
            birdSpawn.gameObject.SetActive(false);
    }
    private void HideEnemies()
    {
        foreach (var enemySpawn in enemySpawns)
            enemySpawn.gameObject.SetActive(false);
    }
}
