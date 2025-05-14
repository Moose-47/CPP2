using UnityEngine;
using UnityEngine.SceneManagement;
public class Bootstrapper : Singleton<Bootstrapper>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BootstrapGame()
    {
        if (GameManager.Instance == null)
        {
            CheckScene("Bootstrap");
        }
        CheckScene("Bootstrap");
    }

    private void Start()
    {
        CheckScene("Title");
    }

    public static void CheckScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
                return;
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
