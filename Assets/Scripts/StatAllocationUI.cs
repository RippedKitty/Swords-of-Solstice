using UnityEngine;
using TMPro;

public class StatAllocationUI : MonoBehaviour
{
    [Header("System References")]
    public CharacterCreationManager charManager;

    [Header("UI Text References")]
    public TextMeshProUGUI availablePointsText;

    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI vitalityText;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI speedText;

    private void OnEnable()
    {
        // Automatically refresh the text the moment the intro state machine 
        // turns this UI panel on (IntroState.StatAllocation)
        UpdateAllStatTexts();
    }

    // --- Button Click Methods ---

    // Note: In the Unity Inspector, you will type the name of the stat 
    // into the string parameter box on the OnClick() event.
    public void IncreaseStat(string statName)
    {
        if (charManager != null)
        {
            charManager.ModifyStat(statName, 1);
            UpdateAllStatTexts();
        }
    }

    public void DecreaseStat(string statName)
    {
        if (charManager != null)
        {
            charManager.ModifyStat(statName, -1);
            UpdateAllStatTexts();
        }
    }

    // --- UI Updating ---

    public void UpdateAllStatTexts()
    {
        if (charManager == null) return;

        // Update the points pool
        availablePointsText.text = $"Points Available: {charManager.availablePoints}";

        // Update the individual stats
        // Because we made these variables public in CharacterCreationManager, we can read them directly
        strengthText.text = charManager.baseStrength.ToString();
        agilityText.text = charManager.baseAgility.ToString();
        vitalityText.text = charManager.baseVitality.ToString();
        intelligenceText.text = charManager.baseIntelligence.ToString();
        speedText.text = charManager.baseSpeed.ToString();
    }
}