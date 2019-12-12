using UnityEngine;

public class BirdsGame : GenericGame
{
    public GameObject[] birdSpawns;

    public override void EndGame()
    {
        SetBirdsActive(false);
    }

    public override void StartGame()
    {
        SetBirdsActive(true);
    }

    private void SetBirdsActive(bool onOff)
    {
        foreach (var birdSpawn in birdSpawns)
            birdSpawn.gameObject.SetActive(onOff);
    }
}
