using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string CheckpointFilePath => Application.persistentDataPath + "/checkpoint.json";
    private string QuickSaveFilePath => Application.persistentDataPath + "/quicksave.json";
    private string CombatSafetyFilePath => Application.persistentDataPath + "/combatSafety.json";

    public GameSaveData currentSaveData = new GameSaveData();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Core Saving ---

    // 1. The Philosopher's Stone Checkpoint (Hard Save)
    public void SaveCheckpoint()
    {
        GatherDataFromManagers();
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(CheckpointFilePath, json);

        // A checkpoint is a safe spot, so clear any combat flags
        SetCombatFlag(false);
        Debug.Log("Philosopher's Stone Checkpoint created.");
    }

    // 2. The standard menu save (Stats and Items between checkpoints)
    public void SaveGame()
    {
        GatherDataFromManagers();
        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(QuickSaveFilePath, json);
        Debug.Log("Standard progress saved.");
    }

    private void GatherDataFromManagers()
    {
        // Pull the current time from the TimeManager
        if (TimeManager.Instance != null)
        {
            currentSaveData.timeData.minute = TimeManager.Instance.currentMinute;
            currentSaveData.timeData.hour = TimeManager.Instance.currentHour;
            currentSaveData.timeData.day = TimeManager.Instance.currentDay;
            currentSaveData.timeData.week = TimeManager.Instance.currentWeek;
            currentSaveData.timeData.month = TimeManager.Instance.currentMonth;
            currentSaveData.timeData.year = TimeManager.Instance.currentYear;
        }

        currentSaveData.statData.baseStr = PlayerStats.Instance.baseStrength;
        currentSaveData.statData.baseAgi = PlayerStats.Instance.baseAgility;
        currentSaveData.statData.baseVit = PlayerStats.Instance.baseVitality;
        currentSaveData.statData.baseInt = PlayerStats.Instance.baseIntelligence;
        currentSaveData.statData.baseSpd = PlayerStats.Instance.baseSpeed;
        currentSaveData.statData.chosenPerkName = PlayerStats.Instance.activePerkName;
    }

    // --- Core Loading & The Anti-Savescum Mechanic ---

    public string BootUpGame()
    {
        // Check if the player force-quit or lost power during a battle
        if (CheckIfDiedOrQuitInCombat())
        {
            Debug.LogWarning("Power outage/Force quit during combat detected! Voiding quicksave. Reverting to Checkpoint.");
            LoadFromFile(CheckpointFilePath);
            return string.Empty;
        }
        else
        {
            // Standard load: Load the quicksave if it exists, otherwise load the checkpoint
            if (File.Exists(QuickSaveFilePath))
            {
                LoadFromFile(QuickSaveFilePath);
                return currentSaveData.lastSavedScene;
            }
            else if (File.Exists(CheckpointFilePath))
            {
                LoadFromFile(CheckpointFilePath);
                return currentSaveData.lastSavedScene;
            }
            else
            {
                Debug.Log("No save files found. Starting new game.");
                return string.Empty;
            }
        }
    }

    private void LoadFromFile(string path)
    {
        string json = File.ReadAllText(path);
        currentSaveData = JsonUtility.FromJson<GameSaveData>(json);

        DistributeDataToManagers();
        Debug.Log($"Loaded data from {path}");
    }

    private void DistributeDataToManagers()
    {
        // Push the loaded time back into the TimeManager
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.currentMinute = currentSaveData.timeData.minute;
            TimeManager.Instance.currentHour = currentSaveData.timeData.hour;
            TimeManager.Instance.currentDay = currentSaveData.timeData.day;
            TimeManager.Instance.currentWeek = currentSaveData.timeData.week;
            TimeManager.Instance.currentMonth = currentSaveData.timeData.month;
            TimeManager.Instance.currentYear = currentSaveData.timeData.year;

            PlayerStats.Instance.baseStrength = currentSaveData.statData.baseStr;
            PlayerStats.Instance.baseAgility = currentSaveData.statData.baseAgi;
            PlayerStats.Instance.baseVitality = currentSaveData.statData.baseVit;
            PlayerStats.Instance.baseIntelligence = currentSaveData.statData.baseInt;
            PlayerStats.Instance.baseSpeed = currentSaveData.statData.baseSpd;
            PlayerStats.Instance.activePerkName = currentSaveData.statData.chosenPerkName;
        }
    }

    // --- Combat Disconnect Prevention ---

    // Call this the exact frame a battle starts: SaveLoadManager.Instance.SetCombatFlag(true);
    // Call this the exact frame a battle is won/escaped: SaveLoadManager.Instance.SetCombatFlag(false);
    public void SetCombatFlag(bool inCombat)
    {
        CombatSafetyData safety = new CombatSafetyData { wasInCombat = inCombat };
        string json = JsonUtility.ToJson(safety);
        File.WriteAllText(CombatSafetyFilePath, json);
    }

    private bool CheckIfDiedOrQuitInCombat()
    {
        if (File.Exists(CombatSafetyFilePath))
        {
            string json = File.ReadAllText(CombatSafetyFilePath);
            CombatSafetyData safety = JsonUtility.FromJson<CombatSafetyData>(json);
            return safety.wasInCombat;
        }
        return false;
    }
}