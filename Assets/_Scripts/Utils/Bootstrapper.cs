using UnityEngine;
using UnityEngine.SceneManagement;
public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    private void Awake()
    {
       if (Instance != null)
       {
            Destroy(gameObject);
            return;
       }

       Instance = this;
        DontDestroyOnLoad(this);

        //create gamemanager
        //create soundmanager
        //create any other managers needed
    }
}

public static class PerformBootstrap
{
    const string BootstrapScenename = "Bootstrap";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void BootstrapGame()
    {
        //Traverse the currently loaded scenes - if it is bootstrap scene exit the function
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == BootstrapScenename)
            {
                return;
            }

            SceneManager.LoadScene(BootstrapScenename, LoadSceneMode.Additive);
        }
    }
}
