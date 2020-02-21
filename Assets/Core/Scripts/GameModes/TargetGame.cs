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
    private int totalArrowsHit;
    public AudioSource targetAppearAudio;
    public AudioSource targetHitAudio;

    private float roundStartedAt;

    [DraggablePoint(false)]
    public Vector3 targetAreaCenter;
    public Vector3 targetAreaSize = Vector3.one;
    private float randomTargetTime = 4;
    private float lastTargetShown = float.MinValue;

    private bool gameEnding;
    private float endGameWaitTime = 5;

    public WarningsController warnings;

    private Coroutine getReadyRoutine;

    private void Start()
    {
        //practiceTarget.onArrowHit += Target_onArrowHit;
        //olympicTarget.onArrowHit += Target_onArrowHit;
        //blitzTarget.onArrowHit += Target_onArrowHit;
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

    //private void Target_onArrowHit(TargetController caller, ArrowController arrow)
    //{
        //PlayHitAt(caller.transform.position);
    //}
    private void Bowman_onArrowShot(ArrowController arrow)
    {
        arrow.onArrowHit += Arrow_onArrowHit;
        StatsViewController.SetArrowsShot(SceneController.sceneControllerInScene.bowman.totalArrowsFired - startArrowsFiredCount);
        StatsViewController.SetArrowsHit(totalArrowsHit);
        if (inGame && GetArrowsLeft() <= 0)
        {
            DoEndGameSequence();
        }
    }
    private void Arrow_onArrowHit(ArrowController caller, TreeCollider.CollisionInfo ci)
    {
        var target = ci.collidedWith.GetComponentInParent<TargetController>();
        if (target != null)
        {
            lastTargetShown = float.MinValue; //If playing random targets, this will make the next target appear

            var scoreTarget = caller.scoreTarget;
            if (scoreTarget != null)
                totalScore += scoreTarget.score;
            else
                Debug.LogError("Could not get score from arrow on target");
            StatsViewController.SetScore(totalScore);

            StatsViewController.SetArrowsHit(++totalArrowsHit);
        }
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
                OptionsPanelController.ShowTargetsPracticeOption(true);

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
            else if (chosenLevel == 2)
            {
                OptionsPanelController.ShowTargetsBlitzOptions(true);
            }
        }
    }
    private void UpdateRandomTargets()
    {
        if (inGame && chosenLevel == 2 && (Time.time - roundStartedAt) > GetCountdownTime())
        {
            if ((Time.time - roundStartedAt) <= GetCountdownTime() + GetRoundTime())
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
            else if (!gameEnding)
                DoEndGameSequence();
        }
        else
            lastTargetShown = float.MinValue;
    }
    private void DoEndGameSequence()
    {
        gameEnding = true;
        SceneController.SetMenuAccess(false);
        SceneController.ShowGameModeMenu(false);
        SceneController.sceneControllerInScene.bowman.SetCanShoot(false);
        StartCoroutine(CommonRoutines.WaitToDoAction((success) =>
        {
            SceneController.EndGameStatic();
            SceneController.ShowGameModeMenu(true);
        }, endGameWaitTime));
        //EndGame();
        //ShowGameOver();
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
        totalArrowsHit = 0;

        startArrowsFiredCount = SceneController.sceneControllerInScene.bowman.totalArrowsFired;
        SceneController.sceneControllerInScene.bowman.onArrowShot += Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        chosenLevel = level;
        SetTargetsActive(true);
        StatsViewController.SetScore(0);

        WaitForReady();

        SceneController.sceneControllerInScene.bowman.SetCanShoot(true);
    }
    private void WaitForReady()
    {
        getReadyRoutine = StartCoroutine(CommonRoutines.WaitToDoAction((isReady) =>
        {
            inGame = true;
            roundStartedAt = Time.time;
        }, 0, () => { return SceneController.sceneControllerInScene.bowman.bowHeld; }));
    }
    public override void EndGame()
    {
        if (getReadyRoutine != null)
            StopCoroutine(getReadyRoutine);

        chosenLevel = -1;

        SceneController.sceneControllerInScene.bowman.SetCanShoot(false);
        SceneController.sceneControllerInScene.bowman.onArrowShot -= Bowman_onArrowShot;
        SceneController.sceneControllerInScene.bowman.DestroyAllArrows();

        SetTargetsActive(false);
        inGame = false;
        gameEnding = false;
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
