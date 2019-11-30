using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController sceneControllerInScene { get; private set; }
    public static bool menuShown { get { return sceneControllerInScene.gameObject.activeSelf; } }
    public bool showMenuOnAwake;

    public GameObject[] scenes;

    private void Awake()
    {
        sceneControllerInScene = this;
        ShowMenu(showMenuOnAwake);
    }

    public static void ShowMenu(bool onOff)
    {
        sceneControllerInScene.gameObject.SetActive(onOff);
    }
    public static void ShowSceneStatic(int sceneIndex)
    {
        for (int i = 0; i < sceneControllerInScene.scenes.Length; i++)
            sceneControllerInScene.scenes[i].SetActive(i == sceneIndex);
    }
    public void ShowScene(int sceneIndex)
    {
        ShowSceneStatic(sceneIndex);
    }
}
