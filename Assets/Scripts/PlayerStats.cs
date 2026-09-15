using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Core Base Stats")]
    public int baseStrength;
    public int baseAgility;
    public int baseVitality;
    public int baseIntelligence;
    public int baseSpeed;

    [Header("Active Perk & Bonuses")]
    public string activePerkName = "None";
    public int perkStrength;
    public int perkAgility;
    public int perkVitality;
    public int perkIntelligence;
    public int perkSpeed;

    [Header("Unallocated Point Multipliers")]
    public float experienceMultiplier = 1.0f;
    public float luckMultiplier = 1.0f;
    public float dropRateMultiplier = 1.0f;

    [Header("Multiplier Settings (Per Point)")]
    [Tooltip("How much % bonus each unallocated point gives. 0.05 = 5%")]
    public float expBonusPerPoint = 0.05f;
    public int luckBonusPerPoint = 1;
    public float dropBonusPerPoint = 0.02f;

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

    // --- Dynamic Stat Getters ---
    // Other scripts will call these to get the TRUE stats for combat damage/health calculations
    public int TotalStrength => baseStrength + perkStrength;
    public int TotalAgility => baseAgility + perkAgility;
    public int TotalVitality => baseVitality + perkVitality;
    public int TotalIntelligence => baseIntelligence + perkIntelligence;

    // --- Initialization (Called at the end of the Intro) ---

    public void InitializeFromCreation(CharacterCreationManager creationData)
    {
        // 1. Set the Base Stats
        baseStrength = creationData.baseStrength;
        baseAgility = creationData.baseAgility;
        baseVitality = creationData.baseVitality;
        baseIntelligence = creationData.baseIntelligence;

        // 2. Apply Perk Bonuses
        if (creationData.finalChosenPerk != null)
        {
            activePerkName = creationData.finalChosenPerk.perkName;
            perkStrength = creationData.finalChosenPerk.bonusStrength;
            perkAgility = creationData.finalChosenPerk.bonusAgility;
            perkVitality = creationData.finalChosenPerk.bonusVitality;
            perkIntelligence = creationData.finalChosenPerk.bonusIntelligence;
        }

        // 3. Calculate Multipliers from Unallocated Points
        int leftoverPoints = creationData.finalUnallocatedPoints;

        // Base is 1.0f (100%). If 4 points left, exp becomes 1.0 + (4 * 0.05) = 1.20f (120%)
        experienceMultiplier = 1.0f + (leftoverPoints * expBonusPerPoint);
        luckMultiplier = 1.0f + (leftoverPoints * luckBonusPerPoint);
        dropRateMultiplier = 1.0f + (leftoverPoints * dropBonusPerPoint);

        Debug.Log($"Stats Initialized! XP Multiplier: {experienceMultiplier}x, Luck: {luckMultiplier}x, Drops: {dropRateMultiplier}x");
    }
}