using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController sceneControllerInScene { get; private set; }
    public static bool menuShown { get { return sceneControllerInScene.gameModeMenu.activeSelf; } }
    public GameObject gameModeMenu, locationMenu;
    public OculusInputController mainInput;

    private int currentScene;
    public WorldData[] scenes;
    public GameObject[] hiddenInGame;
    public BowmanController bowman;

    private void Awake()
    {
        sceneControllerInScene = this;
        ShowGameModeMenu(false);
        ShowLocationMenu(true);
    }

    public static void SetGameModeStatic(WorldData.GameType gameMode)
    {
        sceneControllerInScene.scenes[sceneControllerInScene.currentScene].SetGameMode(gameMode);
        ApplyModeChange();
    }
    public void SetGameMode(WorldData.GameType gameMode)
    {
        SetGameModeStatic(gameMode);
    }
    public void SetGameMode(int gameMode)
    {
        SetGameModeStatic((WorldData.GameType)gameMode);
    }
    public static WorldData.GameType GetCurrentGameMode()
    {
        return sceneControllerInScene.scenes[sceneControllerInScene.currentScene].currentGameMode;
    }

    public static void ApplyModeChange()
    {
        bool inGame = GetCurrentGameMode() != WorldData.GameType.none;
        bool inGameLocation = sceneControllerInScene.currentScene != 0;

        foreach (var hidden in sceneControllerInScene.hiddenInGame)
            hidden.SetActive(!inGame);

        sceneControllerInScene.bowman.gameObject.SetActive(inGame);
        sceneControllerInScene.mainInput.SetMenuAccess(inGameLocation);
        ShowGameModeMenu(inGameLocation);
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

        SetGameModeStatic(WorldData.GameType.none);
    }
    public void ShowScene(int sceneIndex)
    {
        ShowSceneStatic(sceneIndex);
    }
}
