using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController sceneControllerInScene { get; private set; }
    public static bool modesMenuShown { get { return sceneControllerInScene.gameModeMenu.enabled; } }
    public static bool locationsMenuShown { get { return sceneControllerInScene.locationMenu.enabled; } }
    public GameObject uiHelpers;
    public Canvas gameModeMenu, locationMenu;
    public OculusInputController mainInput;
    public HUDController hud;

    private int currentScene;
    public WorldData[] scenes;
    public GameObject[] hiddenInMenu;
    public GameObject[] hiddenInGame;
    public BowmanController bowman;

    public bool canAccessGameModeMenu { get; private set; }

    private void Awake()
    {
        sceneControllerInScene = this;
        ShowGameModeMenu(false);
        ShowLocationMenu(true);
    }
    private void Update()
    {
        UpdateHUD();
        if (mainInput.backButtonDown)
            ShowGameModeMenu(!modesMenuShown);
        uiHelpers.SetActive(modesMenuShown || locationsMenuShown);
    }

    private void UpdateHUD()
    {
        hud.arrowsLeft = scenes[currentScene].GetArrowsLeft();
        hud.score = scenes[currentScene].GetScore();
        hud.startCountdown = scenes[currentScene].GetCountdownTime() - (Time.time - scenes[currentScene].GetRoundTimeStarted());
        hud.roundTimeLeft = (scenes[currentScene].GetRoundTime() + scenes[currentScene].GetCountdownTime()) - (Time.time - scenes[currentScene].GetRoundTimeStarted());
        hud.showGetReady = !scenes[currentScene].GetIsPlaying() && scenes[currentScene].GetRoundTime() > 0;
    }

    public void SetTargetGameRandomInterval(float seconds)
    {
        foreach (var scene in scenes)
            scene.SetTargetGameRandomInterval(seconds);
    }
    public void SetTargetGamePracticeDistance(float meters)
    {
        foreach (var scene in scenes)
            scene.SetTargetGamePracticeDistance(meters);
    }
    public void SetTargetGamePracticeHeight(float meters)
    {
        foreach (var scene in scenes)
            scene.SetTargetGamePracticeHeight(meters);
    }

    public void PlayTargets(int level)
    {
        SetGameModeStatic(WorldData.GameType.targets, level);
    }
    public void PlayBirds(int level)
    {
        SetGameModeStatic(WorldData.GameType.birds, level);
    }
    public void PlayHorde(int level)
    {
        SetGameModeStatic(WorldData.GameType.horde, level);
    }
    public void EndGame()
    {
        Doozy.Engine.GameEventMessage.SendEvent("GotoGameOver");

        sceneControllerInScene.scenes[sceneControllerInScene.currentScene].SetGameMode(WorldData.GameType.none);

        ApplyModeChange();
    }
    public static void SetGameModeStatic(WorldData.GameType gameMode, int level)
    {
        if (gameMode != WorldData.GameType.none)
            Doozy.Engine.GameEventMessage.SendEvent("GotoPause");

        sceneControllerInScene.scenes[sceneControllerInScene.currentScene].SetGameMode(gameMode, level);

        ApplyModeChange();
    }
    public static WorldData.GameType GetCurrentGameMode()
    {
        return sceneControllerInScene.scenes[sceneControllerInScene.currentScene].currentGameMode;
    }

    public void SetMenuAccess(bool onOff)
    {
        canAccessGameModeMenu = onOff;
    }

    private static void ApplyModeChange()
    {
        bool inGame = GetCurrentGameMode() != WorldData.GameType.none;
        bool inGameLocation = sceneControllerInScene.currentScene != 0;

        foreach (var hidden in sceneControllerInScene.hiddenInMenu)
            hidden.SetActive(inGame);
        foreach (var hidden in sceneControllerInScene.hiddenInGame)
            hidden.SetActive(!inGame);

        sceneControllerInScene.bowman.gameObject.SetActive(inGame);
        sceneControllerInScene.SetMenuAccess(inGameLocation);
        ShowGameModeMenu(inGameLocation && !inGame);
        ShowLocationMenu(!inGameLocation);

        OVRManager.fixedFoveatedRenderingLevel = inGame ? OVRManager.FixedFoveatedRenderingLevel.HighTop : OVRManager.FixedFoveatedRenderingLevel.Off; //This seems like an appropriate place
    }
    public static void ShowGameModeMenu(bool onOff)
    {
        sceneControllerInScene.gameModeMenu.enabled = onOff;
        OVREnabler(sceneControllerInScene.gameModeMenu.transform, onOff);
        //sceneControllerInScene.gameModeMenu.sortingOrder = onOff ? 1 : 0;

        //var raycasters = sceneControllerInScene.gameModeMenu.GetComponentsInChildren<OVRRaycaster>();
        //foreach (var raycaster in raycasters)
        //    raycaster.sortOrder = onOff ? 1 : 0;
    }
    public static void ShowLocationMenu(bool onOff)
    {
        sceneControllerInScene.locationMenu.enabled = onOff;
        OVREnabler(sceneControllerInScene.locationMenu.transform, onOff);
        //sceneControllerInScene.locationMenu.sortingOrder = onOff ? 1 : 0;

        //var raycasters = sceneControllerInScene.locationMenu.GetComponentsInChildren<OVRRaycaster>();
        //foreach (var raycaster in raycasters)
        //    raycaster.sortOrder = onOff ? 1 : 0;
    }
    private static void OVREnabler(Transform root, bool onOff)
    {
        var ovrs = root.GetComponentsInChildren<OVRRaycaster>();
        foreach (var ovr in ovrs)
        {
            if (onOff)
                ovr.enabled = ovr.gameObject.activeSelf;
            else
                ovr.enabled = false;
        }
    }

    public static void ShowSceneStatic(int sceneIndex)
    {
        sceneControllerInScene.currentScene = Mathf.Clamp(sceneIndex, 0, sceneControllerInScene.scenes.Length);
        for (int i = 0; i < sceneControllerInScene.scenes.Length; i++)
            sceneControllerInScene.scenes[i].gameObject.SetActive(i == sceneControllerInScene.currentScene);

        SetGameModeStatic(WorldData.GameType.none, 0);
    }
    public void ShowScene(int sceneIndex)
    {
        ShowSceneStatic(sceneIndex);
    }
}
