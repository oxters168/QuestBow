using UnityEngine;

public class TargetGame : GenericGame
{
    public TargetController[] targets;

    private void SetTargetsActive(bool onOff)
    {
        foreach (var target in targets)
            target.gameObject.SetActive(onOff);
    }

    public override void EndGame()
    {
        SetTargetsActive(false);
    }

    public override void StartGame()
    {
        SetTargetsActive(true);
    }
}
