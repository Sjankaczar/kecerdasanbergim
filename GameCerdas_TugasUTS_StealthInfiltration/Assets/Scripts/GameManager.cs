using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Victory, GameOver }
public enum MissionObjective { RetrieveExperiment, ReachExtraction, Complete }

[DefaultExecutionOrder(-1000)]
public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentGameState { get; private set; } = GameState.Playing;
    public MissionObjective CurrentObjective { get; private set; } = MissionObjective.RetrieveExperiment;
    public string CurrentObjectiveDescription => CurrentObjective switch
    {
        MissionObjective.RetrieveExperiment => "Find and retrieve the experimental potion.",
        MissionObjective.ReachExtraction => "Reach the extraction point with the experimental potion.",
        _ => "Mission complete."
    };
    public bool HasExperiment { get; private set; }
    public bool ExtractionAvailable => HasExperiment && CurrentGameState == GameState.Playing;

    public event Action<GameState> GameStateChanged;
    public event Action<MissionObjective> ObjectiveChanged;
    public event Action PotionCollected;
    public event Action OnVictory;
    public event Action OnGameOver;
    public event Action ExtractionDenied;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        BeginSession();
    }

    private void OnDestroy()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        if (Instance == this) Instance = null;
    }

    private void OnSceneUnloaded(Scene scene) { ResetSession(); }

    private void BeginSession()
    {
        Time.timeScale = 1f;
        HasExperiment = false;
        CurrentObjective = MissionObjective.RetrieveExperiment;
        SetState(GameState.Playing);
        ObjectiveChanged?.Invoke(CurrentObjective);
    }

    private void ResetSession()
    {
        Time.timeScale = 1f;
        HasExperiment = false;
        CurrentObjective = MissionObjective.RetrieveExperiment;
        CurrentGameState = GameState.Playing;
    }

    public bool TryCollectExperiment()
    {
        if (CurrentGameState != GameState.Playing || HasExperiment || CurrentObjective != MissionObjective.RetrieveExperiment) return false;
        HasExperiment = true;
        PotionCollected?.Invoke();
        CurrentObjective = MissionObjective.ReachExtraction;
        ObjectiveChanged?.Invoke(CurrentObjective);
        return true;
    }

    public bool TryExtract()
    {
        if (CurrentGameState != GameState.Playing) return false;
        if (!HasExperiment || CurrentObjective != MissionObjective.ReachExtraction) { ExtractionDenied?.Invoke(); return false; }
        CurrentObjective = MissionObjective.Complete;
        ObjectiveChanged?.Invoke(CurrentObjective);
        SetState(GameState.Victory);
        OnVictory?.Invoke();
        Debug.Log("MISSION COMPLETE");
        return true;
    }

    public bool CapturePlayer()
    {
        if (CurrentGameState != GameState.Playing) return false;
        Time.timeScale = 1f;
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
        Debug.Log("MISSION FAILED");
        return true;
    }

    private void SetState(GameState state)
    {
        if (CurrentGameState == state || CurrentGameState == GameState.Victory || CurrentGameState == GameState.GameOver) return;
        CurrentGameState = state;
        GameStateChanged?.Invoke(state);
    }

    public bool SetPaused(bool paused)
    {
        if (CurrentGameState == GameState.Victory || CurrentGameState == GameState.GameOver) return false;
        GameState next = paused ? GameState.Paused : GameState.Playing;
        if (CurrentGameState == next) return true;
        if (paused && CurrentGameState != GameState.Playing) return false;
        if (!paused && CurrentGameState != GameState.Paused) return false;
        CurrentGameState = next;
        Time.timeScale = paused ? 0f : 1f;
        GameStateChanged?.Invoke(next);
        return true;
    }

    public void RestartMission()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        EnemyFSM.StopAllAlertAudioForSceneChange();
        SceneManager.LoadScene("MainMenu");
    }
}
