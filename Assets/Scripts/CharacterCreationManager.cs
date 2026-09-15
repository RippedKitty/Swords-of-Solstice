using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public class StarterPerk
{
    public string perkName;
    [TextArea(2, 3)] public string description;

    [Header("Bonus Stats")]
    public int bonusStrength;
    public int bonusAgility;
    public int bonusVitality;
    public int bonusIntelligence;
    public int bonusSpeed;
}

public class CharacterCreationManager : MonoBehaviour
{
    [Header("System References")]
    public IntroSequenceManager introManager;

    [Header("Perk Selection")]
    public List<StarterPerk> availablePerks;
    public StarterPerk finalChosenPerk;
    private bool perkIsConfirmed = false;

    [Header("Stat Allocation")]
    public int totalStatPoints = 20;
    public int availablePoints; // Points left to spend
    public int finalUnallocatedPoints; // Points saved for XP/Luck bonuses

    [Header("Current Base Stats")]
    // The raw stats before perk bonuses are applied
    public int baseStrength = 5;
    public int baseAgility = 5;
    public int baseVitality = 5;
    public int baseIntelligence = 5;
    public int baseSpeed = 5;

    void Start()
    {
        availablePoints = totalStatPoints;
    }

    void Update()
    {
        // Allow the player to hit Escape to pick a random perk and move on
        if (introManager != null && introManager.currentState == IntroSequenceManager.IntroState.PerkSelection)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                ConfirmPerkSelection(true);
            }
        }
    }

    // --- Perk Selection Methods (Link these to UI Buttons) ---

    // Pass the index of the perk from your UI button (0, 1, 2, etc.)
    public void SelectPerk(int index)
    {
        if (index >= 0 && index < availablePerks.Count)
        {
            finalChosenPerk = availablePerks[index];
            Debug.Log("Selected Perk: " + finalChosenPerk.perkName);
        }
    }

    public void SelectRandomPerk()
    {
        int randomIndex = Random.Range(0, availablePerks.Count);
        finalChosenPerk = availablePerks[randomIndex];
        Debug.Log("Randomly Selected Perk: " + finalChosenPerk.perkName);
    }

    // Call this from a "Confirm Perk" UI Button
    public void ConfirmPerkSelection(bool isRandom)
    {
        if (isRandom)
        {
            SelectRandomPerk();
        }

        if (finalChosenPerk != null)
        {
            perkIsConfirmed = true;
            introManager.OnPerkConfirmed(isRandom);
        }
        else
        {
            Debug.LogWarning("No perk selected yet!");
        }
    }

    // --- Stat Allocation Methods (Link these to + / - UI Buttons) ---

    // Example usage: ModifyStat("Strength", 1) or ModifyStat("Agility", -1)
    public void ModifyStat(string statName, int amount)
    {
        // Prevent spending points we don't have
        if (amount > 0 && availablePoints - amount < 0) return;

        bool statChanged = false;

        switch (statName.ToLower())
        {
            case "strength":
                if (baseStrength + amount >= 1) // Assuming 1 is the minimum stat
                {
                    baseStrength += amount;
                    statChanged = true;
                }
                break;
            case "agility":
                if (baseAgility + amount >= 1)
                {
                    baseAgility += amount;
                    statChanged = true;
                }
                break;
            case "vitality":
                if (baseVitality + amount >= 1)
                {
                    baseVitality += amount;
                    statChanged = true;
                }
                break;
            case "intelligence":
                if (baseIntelligence + amount >= 1)
                {
                    baseIntelligence += amount;
                    statChanged = true;
                }
                break;
            case "speed":
                if (baseSpeed + amount >= 1)
                { 
                    baseSpeed += amount;
                    statChanged = true;
                }
                break;
        }

        if (statChanged)
        {
            availablePoints -= amount;
            // Note: You will tell your UI script to update the text fields here
            Debug.Log($"{statName} is now {GetBaseStat(statName)}. Points left: {availablePoints}");
        }
    }

    private int GetBaseStat(string statName)
    {
        switch (statName.ToLower())
        {
            case "strength": return baseStrength;
            case "agility": return baseAgility;
            case "vitality": return baseVitality;
            case "intelligence": return baseIntelligence;
            case "speed": return baseSpeed;
            default: return 0;
        }
    }

    // Call this from a "Confirm Build" UI Button
    public void ConfirmStatAllocation()
    {
        // Save the leftover points to calculate XP/Luck/Drop bonuses later
        finalUnallocatedPoints = availablePoints;

        Debug.Log($"Stats confirmed. Unallocated points saved for multipliers: {finalUnallocatedPoints}");

        introManager.OnStatsConfirmed();
    }
}