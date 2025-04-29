using System.Collections.Generic;       
using UnityEngine;                      
using UnityEngine.SceneManagement;     

//This script manages all menus in the game and switches between them
public class MenuController : MonoBehaviour
{
    public BaseMenu[] allMenus;                //A list of all the menu scripts in the scene
    public MenuStates initState = MenuStates.MainMenu;  //The menu state to start with
    private BaseMenu currentState;             //The menu that's currently active

    Dictionary<MenuStates, BaseMenu> menuDictionary = new Dictionary<MenuStates, BaseMenu>(); //Fast lookup for menus by state
    Stack<MenuStates> menuStack = new Stack<MenuStates>(); //Keeps track of menu history for jumping back

    private void Start()
    {
        //If no menus were assigned manually, automatically find them in child objects (even if disabled)
        if (allMenus.Length <= 0)
        {
            allMenus = gameObject.GetComponentsInChildren<BaseMenu>(true);
        }

        //Set up each menu
        foreach (BaseMenu menu in allMenus)
        {
            if (menu == null) continue;        //Skip any empty entries
            menu.Init(this);                   //Give the menu a reference to this controller

            if (menuDictionary.ContainsKey(menu.state)) continue; //Skip if already added

            menuDictionary.Add(menu.state, menu); //Add the menu to the dictionary
        }

        SetActiveState(initState);             //Start with the initial menu (default is MainMenu)
        GameManager.Instance.SetMenuController(this); //Let the GameManager know about this controller
    }

    //This goes back to the previous menu screen
    public void JumpBack()
    {
        if (menuStack.Count <= 0) return;      //Do nothing if there's no menu to go back to

        menuStack.Pop();                       //Remove the current menu from the history

        if (menuStack.Count > 0)
        {
            SetActiveState(menuStack.Peek(), true); //Go back to the previous menu (without adding it again to the stack)
        }
        else
        {
            //If there's no more history, disable the current menu
            if (currentState != null)
            {
                currentState.ExitState();
                currentState.gameObject.SetActive(false);
                currentState = null; // No active menu now
            }
        }
    }

    //Switch to a new menu
    public void SetActiveState(MenuStates newState, bool isJumpingBack = false)
    {
        if (!menuDictionary.ContainsKey(newState)) return; //Do nothing if we don't know this menu
        if (currentState == menuDictionary[newState]) return; //If it's already active, don't switch

        //Turn off the current menu
        if (currentState != null)
        {
            currentState.ExitState();                 //Run any exit logic
            currentState.gameObject.SetActive(false); //Hide it
        }

        // Activate the new menu
        currentState = menuDictionary[newState];
        currentState.gameObject.SetActive(true);      //Show it
        currentState.EnterState();                    //Run any enter logic

        //If this wasn't triggered by JumpBack, record it in the history
        if (!isJumpingBack) menuStack.Push(newState);
    }

    // --- The following code handles pause/resume functionality ---

    //Toggles the pause menu on/off
    private void TogglePauseMenu()
    {
        if (currentState != null && currentState.state == MenuStates.Pause)
        {
            ResumeGame(); // If we're already paused, resume
        }
        else
        {
            PauseGame();  // Otherwise, pause the game
        }
    }

    //Pauses the game and shows the pause menu
    private void PauseGame()
    {
        SetActiveState(MenuStates.Pause);
        Time.timeScale = 0f; // Freezes game time
    }

    //Resumes the game from pause
    public void ResumeGame()
    {
        JumpBack(); // Go back to the previous menu

        //Make sure the settings menu is closed (in case we came from it)
        if (menuDictionary.ContainsKey(MenuStates.Settings))
        {
            BaseMenu settingsMenu = menuDictionary[MenuStates.Settings];
            settingsMenu.ExitState();
            settingsMenu.gameObject.SetActive(false);
        }

        Time.timeScale = 1f; // Unfreezes game time
    }

    //Check for key press to toggle pause menu (only in the Game scene)
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                TogglePauseMenu();
            }
        }
    }

}
