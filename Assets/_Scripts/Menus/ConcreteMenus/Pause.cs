using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : BaseMenu
{
    public Button quitBtn;
    public Button resumeBtn;
    public Button mainMenuBtn;
    public Button settingsBtn;

    public override void Init(MenuController context)
    {
        base.Init(context);
        state = MenuStates.Pause;

        if (resumeBtn) resumeBtn.onClick.AddListener(context.ResumeGame);
        if (mainMenuBtn) mainMenuBtn.onClick.AddListener(() => GameManager.Instance.ChangeScene("Title"));
        if (settingsBtn) settingsBtn.onClick.AddListener(() => SetNextMenu(MenuStates.Settings));
        if (quitBtn) quitBtn.onClick.AddListener(QuitGame);
    }
}
