using UnityEngine;

public class SceneController : MonoBehaviour
{
    private AsyncOperation sceneLoadOperation;
    public UnityEngine.UI.Image progressBar;

    private void Update()
    {
        if (sceneLoadOperation != null)
        {
            progressBar.fillAmount = sceneLoadOperation.progress;
        }
    }

    public void LoadScene(int sceneIndex)
    {
        sceneLoadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, UnityEngine.SceneManagement.LoadSceneMode.Single);
        sceneLoadOperation.completed += SceneLoadOperation_completed;
    }

    private void SceneLoadOperation_completed(AsyncOperation obj)
    {
        gameObject.SetActive(false);
        sceneLoadOperation.completed -= SceneLoadOperation_completed;
    }
}
