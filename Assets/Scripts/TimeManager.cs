using UnityEngine;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Calendar Settings (Solstice)")]
    private const int HoursPerDay = 24;
    private const int MinutesPerHour = 60;
    private const int DaysPerWeek = 5;
    private const int WeeksPerMonth = 5;
    private const int MonthsPerYear = 4;

    [Header("Current Time")]
    public int currentMinute = 0;
    public int currentHour = 6; // Start at 6:00 AM
    public int currentDay = 1;
    public int currentWeek = 1;
    public int currentMonth = 1;
    public int currentYear = 1;

    [Header("Demon Lord Invasion")]
    public int invasionYear = 6; // Invades at the very start of Year 6 (exactly 5 full years)
    public int totalDaysRemaining;

    [Header("Time Flow Settings")]
    public bool isTimePaused = true; // Kept paused during the intro scene
    public float realSecondsPerInGameMinute = 2f; // How fast time actually passes while playing
    private float timeAccumulator = 0f;

    // Events that other scripts can subscribe to
    public event Action OnMinuteChanged;
    public event Action OnHourChanged;
    public event Action OnDayChanged;
    public event Action OnSeasonChanged; // Tied to month change
    public event Action OnYearChanged;
    public event Action OnInvasionTriggered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keeps the calendar alive across all scenes

        CalculateDaysRemaining();
    }

    private void Update()
    {
        if (isTimePaused) return;

        timeAccumulator += Time.deltaTime;

        if (timeAccumulator >= realSecondsPerInGameMinute)
        {
            timeAccumulator -= realSecondsPerInGameMinute;
            AdvanceMinute();
        }
    }

    // --- Time Cascading Logic ---

    private void AdvanceMinute()
    {
        currentMinute++;
        OnMinuteChanged?.Invoke();

        if (currentMinute >= MinutesPerHour)
        {
            currentMinute = 0;
            AdvanceHour();
        }
    }

    private void AdvanceHour()
    {
        currentHour++;
        OnHourChanged?.Invoke();

        if (currentHour >= HoursPerDay)
        {
            currentHour = 0;
            AdvanceDay();
        }
    }

    private void AdvanceDay()
    {
        currentDay++;
        CalculateDaysRemaining();
        OnDayChanged?.Invoke();

        if (currentDay > DaysPerWeek)
        {
            currentDay = 1;
            AdvanceWeek();
        }
    }

    private void AdvanceWeek()
    {
        currentWeek++;

        if (currentWeek > WeeksPerMonth)
        {
            currentWeek = 1;
            AdvanceMonth();
        }
    }

    private void AdvanceMonth()
    {
        currentMonth++;
        OnSeasonChanged?.Invoke();

        if (currentMonth > MonthsPerYear)
        {
            currentMonth = 1;
            AdvanceYear();
        }
    }

    private void AdvanceYear()
    {
        currentYear++;
        OnYearChanged?.Invoke();

        if (currentYear >= invasionYear)
        {
            TriggerInvasion();
        }
    }

    // --- Game Logic ---

    private void CalculateDaysRemaining()
    {
        // 1 Year = 4 Months. 1 Month = 5 Weeks. 1 Week = 5 Days. 
        // Total = 100 days per year.
        int daysPerYear = DaysPerWeek * WeeksPerMonth * MonthsPerYear;

        int targetTotalDays = (invasionYear - 1) * daysPerYear;

        int currentTotalDays = ((currentYear - 1) * daysPerYear) +
                               ((currentMonth - 1) * (DaysPerWeek * WeeksPerMonth)) +
                               ((currentWeek - 1) * DaysPerWeek) +
                               currentDay;

        totalDaysRemaining = targetTotalDays - currentTotalDays + 1;

        if (totalDaysRemaining <= 0 && currentYear >= invasionYear)
        {
            totalDaysRemaining = 0;
        }
    }

    private void TriggerInvasion()
    {
        isTimePaused = true;
        OnInvasionTriggered?.Invoke();
        Debug.Log("The Demon Lord has invaded the mainland. Time is up.");
    }
}