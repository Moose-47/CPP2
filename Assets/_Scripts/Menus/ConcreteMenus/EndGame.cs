using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : BaseMenu
{
    public Button mainMenuBtn;
    public Button quitBtn;

    public TMP_Text fliesCollectedTxt;
    public TMP_Text beetlesCollectedTxt;
    public TMP_Text score;
    public override void Init(MenuController context)
    {
        base.Init(context);
        state = MenuStates.EndScreen;

        if (mainMenuBtn) mainMenuBtn.onClick.AddListener(() => GameManager.Instance.ChangeScene("Title"));
        if (quitBtn) quitBtn.onClick.AddListener(QuitGame);
    }
}