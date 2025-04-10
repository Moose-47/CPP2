using UnityEngine;
using UnityEngine.SceneManagement;
public class EndscreenTemp : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Game");
        }
    }
}
