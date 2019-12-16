using UnityEngine;

public class HordeGame : GenericGame
{
    public GameObject[] enemySpawns;
    private bool inGame;

    public override void EndGame()
    {
        SetEnemiesActive(false);
        inGame = false;
    }

    public override void StartGame(int level)
    {
        SetEnemiesActive(true);
        inGame = true;

        SceneController.sceneControllerInScene.bowman.canShoot = true;
    }

    public override bool IsPlaying()
    {
        return inGame;
    }

    public override int GetArrowsLeft()
    {
        return 1;
    }
    public override int GetScore()
    {
        return 0;
    }

    private void SetEnemiesActive(bool onOff)
    {
        foreach (var enemySpawn in enemySpawns)
            enemySpawn.gameObject.SetActive(onOff);
    }
}
