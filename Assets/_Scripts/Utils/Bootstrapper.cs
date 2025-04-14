using UnityEngine;
using UnityEngine.SceneManagement;
public class Bootstrapper : Singleton<Bootstrapper>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BootstrapGame()
    {
        CheckScene("Bootstrap");
    }

    private void Start()
    {
        {
            CheckScene("Game");
        }
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
