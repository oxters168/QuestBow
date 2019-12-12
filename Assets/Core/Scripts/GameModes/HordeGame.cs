using UnityEngine;

public class HordeGame : GenericGame
{
    public GameObject[] enemySpawns;

    public override void EndGame()
    {
        SetEnemiesActive(false);
    }

    public override void StartGame()
    {
        SetEnemiesActive(true);
    }

    private void SetEnemiesActive(bool onOff)
    {
        foreach (var enemySpawn in enemySpawns)
            enemySpawn.gameObject.SetActive(onOff);
    }
}
