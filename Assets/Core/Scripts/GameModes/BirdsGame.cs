﻿using UnityEngine;

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

    private void SetBirdsActive(bool onOff)
    {
        foreach (var birdSpawn in birdSpawns)
            birdSpawn.gameObject.SetActive(onOff);
    }
}