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

    private void OnDestroy()
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Destroy(gameManager);
        }
    }
    private void OnApplicationQuit()
    {
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            Destroy(gameManager.gameObject);
        }
    }
}
