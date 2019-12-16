using UnityEngine;

public abstract class GenericGame : MonoBehaviour
{
    public abstract void StartGame(int level);
    public abstract void EndGame();
    public abstract bool IsPlaying();
    public abstract int GetArrowsLeft();
    public abstract int GetScore();
}
