using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class GameStateManager : Singleton<GameStateManager>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public enum GameState
    {
        Setup,
        // Game states/loop -->
        Gameplay,
        // <--
        Win,
        Lose,
        Exiting
    }
    public static GameState State { get; private set; }


    private void Start() => ChangeState(GameState.Setup);

    public static void ChangeState(GameState newGameState)
    {
        if (State == newGameState)
            return;

        OnBeforeStateChanged?.Invoke(newGameState);

        State = newGameState;

        switch(newGameState)
        {
            case GameState.Setup:
                // Anything to set up the scene ready for gameplay
                HandleSetup();
                break;
            case GameState.Gameplay:
                // Anything that happens during gameplay. It is likely that more
                // states should replace this one to account for game complexity.
                HandleGameplay();
                break;
            case GameState.Win:
                // Anything that should happen when the player meets the win
                // condition of the game.
                HandleGameWin();
                break;
            case GameState.Lose:
                // Anything that should happen when the player meets the lose
                // condition of the game.
                HandleGameLose();
                break;
            case GameState.Exiting:
                // Anything that should happen as the game exits.
                HandleExit();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
        }

        Debug.Log("Game state changed to " + newGameState.ToString());
        OnAfterStateChanged?.Invoke(newGameState);
    }

    private static void HandleSetup()
    {
        // Anything to set up the scene ready for gameplay.

        ChangeState(GameState.Gameplay);
    }

    private static void HandleGameplay()
    {
        // Anything that happens during gameplay. It is likely that more
        // states should replace this one to account for game complexity.
        // Check if Win/Lose conditions are met.

    }

    private static void HandleGameWin()
    {
        // Anything that should happen when the player meets the win
        // condition of the game.

    }

    private static void HandleGameLose()
    {
        // Anything that should happen when the player meets the lose
        // condition of the game.

    }

    private static void HandleExit()
    {
        // Anything that should happen as the game exits.

        Application.Quit();
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
    }
}
