using UnityEngine;

public class BirdsGame : GenericGame
{
    public GameObject[] birdSpawns;
    private bool inGame;

    public override void EndGame()
    {
        SetBirdsActive(false);
        inGame = false;
    }
    public override void StartGame(int level)
    {
        SetBirdsActive(true);
        inGame = true;

        SceneController.sceneControllerInScene.bowman.SetCanShoot(true);
    }
    public override bool IsPlaying()
    {
        return inGame;
    }
    public override int GetLevelArrowCount()
    {
        return 1;
    }
    public override int GetArrowsLeft()
    {
        return GetLevelArrowCount();
    }
    public override int GetScore()
    {
        return 0;
    }

    private void SetBirdsActive(bool onOff)
    {
        foreach (var birdSpawn in birdSpawns)
            birdSpawn.gameObject.SetActive(onOff);
    }

    public override float GetRoundStartTime()
    {
        return 0;
    }

    public override float GetCountdownTime()
    {
        return 0;
    }

    public override float GetRoundTime()
    {
        return -1;
    }
}
