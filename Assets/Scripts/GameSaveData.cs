using System;

[Serializable]
public class GameSaveData
{
    public string lastSavedScene;
    public TimeData timeData = new TimeData();
    public PlayerStatData statData = new PlayerStatData();
}

[Serializable]
public class TimeData
{
    public int minute, hour, day, week, month, year;
}

[Serializable]
public class PlayerStatData
{
    // Base stats, perks, and leftover multipliers
    public int baseStr, baseAgi, baseVit, baseInt, baseSpd;
    public string chosenPerkName;
    public int unallocatedPoints;
}

// This tiny file is saved instantly when combat starts to prevent save-scumming
[Serializable]
public class CombatSafetyData
{
    public bool wasInCombat;
}