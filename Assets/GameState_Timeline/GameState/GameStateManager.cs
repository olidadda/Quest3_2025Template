using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance; // Singleton for easy access

    public enum GameState { Lobby, SceneA, SceneB, Pause, EndGame }
    public enum MenuState { None, MainMenu, Options, Credits, Loading }

    [SerializeField] private bool hasConfirmedIDNumber = false;

    [SerializeField] private int iDNumber = 0;  
    

    [Header("Current States")]
    public GameState currentGameState = GameState.Lobby;
    public MenuState currentMenuState = MenuState.None;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        Instance = this;

        //DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    public void SetGameState(GameState newState)
    {
        Debug.Log($"Game State changed to: {newState}");
        currentGameState = newState;
    }

    public void SetMenuState(MenuState newState)
    {
        Debug.Log($"Menu State changed to: {newState}");
        currentMenuState = newState;
    }

    public bool IsGameState(GameState state) => currentGameState == state;
    public bool IsMenuState(MenuState state) => currentMenuState == state;

    public void SetIDNumberAsAlreadyChosen(int iDnum)
    {
        hasConfirmedIDNumber |= true;
        iDNumber = iDnum;
    }
}

