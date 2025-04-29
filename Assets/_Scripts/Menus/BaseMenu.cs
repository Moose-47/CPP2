using UnityEngine;

//This defines a list of possible menu states the game can be in
public enum MenuStates
{
    MainMenu,     //The main menu screen
    Settings,     //The settings/options screen
    Pause,        //The pause menu shown during gameplay
    InGame,       //The active gameplay state
    Credits,      //The screen that shows game credits
    GameOver,     //The screen shown after the player loses
    EndScreen     //The screen shown at the end of the game
}

//This class serves as a base (template) for all menu screens
public class BaseMenu : MonoBehaviour
{
    [HideInInspector]
    public MenuStates state; //Keeps track of which state this menu represents (not visible in Inspector)

    protected MenuController context; //A reference to the MenuController that manages all menus

    //This method sets the context (MenuController) so the menu knows who controls it
    public virtual void Init(MenuController context) => this.context = context;

    //This method runs when the menu becomes active (can be overridden by child classes)
    public virtual void EnterState() { }

    //This method runs when the menu is no longer active (can be overridden by child classes)
    public virtual void ExitState() { }

    //This method quits the game — behaves differently in the Unity Editor vs a built game
    public void QuitGame()
    {
#if UNITY_EDITOR
        //If we're in the Unity Editor, just stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If it's a built game, actually quit the application
        Application.Quit();
#endif
    }

    //This tells the MenuController to go back to the previous menu
    public void JumpBack() => context.JumpBack();

    //This tells the MenuController to switch to a new menu state
    public void SetNextMenu(MenuStates newState) => context.SetActiveState(newState);
}
