using UnityEngine;
using UnityEngine.UI;

public class creditsMenu : BaseMenu
{
    public Button backBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Init(MenuController context)
    {
        base.Init(context);
        state = MenuStates.Credits;

        if (backBtn) backBtn.onClick.AddListener(JumpBack);
    }
}
