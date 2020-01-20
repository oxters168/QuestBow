using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController sceneControllerInScene { get; private set; }
    public static bool menuShown { get { return sceneControllerInScene.gameModeMenu.activeSelf; } }
    public GameObject gameModeMenu, locationMenu;
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
            ShowGameModeMenu(!menuShown);
    }

    private void UpdateHUD()
    {
        hud.arrowsLeft = scenes[currentScene].GetArrowsLeft();
        hud.score = scenes[currentScene].GetScore();
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
    public static void SetGameModeStatic(WorldData.GameType gameMode, int level)
    {
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

    public static void ApplyModeChange()
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
    }
    public static void ShowGameModeMenu(bool onOff)
    {
        sceneControllerInScene.gameModeMenu.SetActive(onOff);
    }
    public static void ShowLocationMenu(bool onOff)
    {
        sceneControllerInScene.locationMenu.SetActive(onOff);
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
