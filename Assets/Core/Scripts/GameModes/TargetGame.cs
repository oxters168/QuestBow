using UnityEngine;
using UnityHelpers;

public class TargetGame : GenericGame
{
    private readonly GameVariables[] levelVariables = new GameVariables[]
    {
        new GameVariables() { name = "Practice", arrows = -1, countdownTime = 0, roundTime = -1 },
        new GameVariables() { name = "Olympic", arrows = 6, countdownTime = 0, roundTime = -1 },
        new GameVariables() { name = "Blitz", arrows = -1, countdownTime = 3, roundTime = 30 }
    };
    public TargetController practiceTarget;
    public TargetController olympicTarget;
    public TargetController blitzTarget;
    private int chosenLevel;
    private bool inGame;
    public int totalScore { get; private set; }
    private int startArrowsFiredCount;
    public AudioSource targetAppearAudio;
    public AudioSource targetHitAudio;

    private float roundStartedAt;

    [DraggablePoint(false)]
    public Vector3 targetAreaCenter;
    public Vector3 targetAreaSize = Vector3.one;
    private float randomTargetTime = 4;
    private float lastTargetShown = float.MinValue;

    public WarningsController warnings;

    Coroutine getReadyRoutine;

    private void Start()
    {
        practiceTarget.onArrowHit += Target_onArrowHit;
        olympicTarget.onArrowHit += Target_onArrowHit;
        blitzTarget.onArrowHit += Target_onArrowHit;

        //warnings.TrackObject(blitzTarget, 0, false);
    }
    private void Update()
    {
        UpdateRandomTargets();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(targetAreaCenter, targetAreaSize);
    }

    private void Target_onArrowHit(TargetController caller, ArrowController arrow)
    {
        lastTargetShown = float.MinValue; //If playing random targets, this will make the next target appear

        totalScore += arrow.scoreTarget.score;
        //PlayHitAt(caller.transform.position);
    }

    private void SetTargetsActive(bool onOff)
    {
        practiceTarget.gameObject.SetActive(false);
        olympicTarget.gameObject.SetActive(false);
        blitzTarget.gameObject.SetActive(false);

        if (onOff)
        {
            if (chosenLevel == 0)
            {
                practiceTarget.transform.forward = Vector3.forward;
                SetPracticeDistance(10);
                SetPracticeHeight(1);
                practiceTarget.gameObject.SetActive(true);
                //PlayAppearedAt(practiceTarget.transform.position);
            }
            else if (chosenLevel == 1)
            {
                olympicTarget.gameObject.SetActive(true);
                //PlayAppearedAt(olympicTarget.transform.position);
            }
        }
    }
    private void UpdateRandomTargets()
    {
        if (inGame && chosenLevel == 2 && (Time.time - roundStartedAt) > GetCountdownTime())
        {
            if (Time.time - lastTargetShown >= randomTargetTime)
            {
                blitzTarget.transform.position = new Vector3(Random.Range(-1f, 1f) * targetAreaSize.x / 2, Random.Range(-1f, 1f) * targetAreaSize.y / 2, Random.Range(-1f, 1f) * targetAreaSize.z / 2) + targetAreaCenter;
                blitzTarget.transform.forward = (SceneController.sceneControllerInScene.bowman.transform.position - blitzTarget.transform.position).normalized;
                blitzTarget.gameObject.SetActive(true);
                PlayAppearedAt(blitzTarget.transform.position);
                lastTargetShown = Time.time;
            }
        }
        else
            lastTargetShown = float.MinValue;
    }
    public void SetRandomTargetInterval(float seconds)
    {
        randomTargetTime = seconds;
    }

    private void PlayAppearedAt(Vector3 position)
    {
        targetAppearAudio.transform.position = position;
        targetAppearAudio.Play();
    }
    private void PlayHitAt(Vector3 position)
    {
        targetHitAudio.transform.position = position;
        targetHitAudio.Play();
    }

    public void SetPracticeDistance(float meters)
    {
        float height = practiceTarget.transform.position.y;
        practiceTarget.transform.position = SceneController.sceneControllerInScene.bowman.transform.position - practiceTarget.transform.forward * meters + Vector3.up * height;
    }
    public void SetPracticeHeight(float meters)
    {
        practiceTarget.transform.position = new Vector3(practiceTarget.transform.position.x, meters, practiceTarget.transform.position.z);
    }

    public override void StartGame(int level)
    {
        totalScore = 0;

        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;
        SceneController.sceneControllerInScene.bowman.onArrowShot += Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        chosenLevel = level;
        SetTargetsActive(true);

        getReadyRoutine = StartCoroutine(CommonRoutines.WaitToDoAction((isReady) =>
        {
            inGame = true;
            roundStartedAt = Time.time;
        }, 0, () => { return SceneController.sceneControllerInScene.bowman.bowHeld; }));

        SceneController.sceneControllerInScene.bowman.canShoot = true;
    }
    public override void EndGame()
    {
        if (getReadyRoutine != null)
            StopCoroutine(getReadyRoutine);

        chosenLevel = -1;

        SceneController.sceneControllerInScene.bowman.onArrowShot -= Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        SetTargetsActive(false);
        inGame = false;
    }
    public override int GetLevelArrowCount()
    {
        int arrowCount = 0;
        if (chosenLevel >= 0)
            arrowCount = levelVariables[chosenLevel].arrows;
        return arrowCount;
    }
    public override int GetArrowsLeft()
    {
        int arrowsLeft = GetLevelArrowCount();
        if (arrowsLeft >= 0)
            arrowsLeft -= SceneController.sceneControllerInScene.bowman.totalArrowsFired - startArrowsFiredCount;
        else
            arrowsLeft = int.MaxValue;

        return arrowsLeft;
    }
    public override int GetScore()
    {
        return totalScore;
    }

    private void Bowman_onArrowShot()
    {
        if (inGame && GetArrowsLeft() <= 0)
            SceneController.sceneControllerInScene.bowman.canShoot = false;
    }

    public override bool IsPlaying()
    {
        return inGame;
    }

    public override float GetRoundStartTime()
    {
        return roundStartedAt;
    }
    public override float GetCountdownTime()
    {
        float countdownTime = 0;
        if (chosenLevel >= 0)
            countdownTime = levelVariables[chosenLevel].countdownTime;
        return countdownTime;
    }
    public override float GetRoundTime()
    {
        float roundTime = -1;
        if (chosenLevel >= 0)
            roundTime = levelVariables[chosenLevel].roundTime;
        return roundTime;
    }
}
