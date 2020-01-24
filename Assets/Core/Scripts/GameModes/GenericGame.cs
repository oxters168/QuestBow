using UnityEngine;

public abstract class GenericGame : MonoBehaviour
{
    public abstract void StartGame(int level);
    public abstract void EndGame();
    public abstract bool IsPlaying();
    public abstract int GetArrowsLeft();
    public abstract int GetLevelArrowCount();
    public abstract int GetScore();
    public abstract float GetRoundStartTime();
    public abstract float GetCountdownTime();
    public abstract float GetRoundTime();
}

public struct GameVariables
{
    public string name;
    public int arrows;
    public float countdownTime;
    public float roundTime;
}