using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroSequenceManager : MonoBehaviour
{
    // The state machine defining the flow of the intro
    public enum IntroState
    {
        Initialize,
        PlayOpeningAnimation,
        FadeCharacterIn,
        PanToGodess,
        GoddessDialogue1,
        PerkSelection,
        GoddessDialogue2,
        PanToStatAllocation,
        StatAllocation,
        GoddessDialogue3,
        PanToPortal,
        FinalizeAndLoad
    }

    [Header("Current State")]
    public IntroState currentState;

    [Header("UI & System References")]
    public IntroCameraPanner cameraPanner;
    public DialogueController dialogueController;
    public CharacterCreationManager charCreationManager;
    public GameObject dialogueUI;
    public GameObject perkSelectionUI;
    public GameObject statAllocationUI;

    [Header("Opening Animation (Animator)")]
    public Animator introAnimator; // Drag your screen-covering Animator here
    public string introAnimationTrigger = "PlayIntro"; // The trigger parameter in your Animator Controller
    public float openingAnimationDuration = 3.0f; // Set this to exactly match your animation clip length

    [Header("Fade Settings")]
    public float fadeStepDelay = 0.5f;
    public float fadeStepAmount = 0.1f;
    public float animationFadeOutDuration = 1.0f;

    [Header("Scene Transition")]
    public string mainWorldSceneName = "SolsticeWorld";

    [Header("Character Reveal")]
    public SpriteRenderer playerSprite; // Or use CanvasGroup if your player is a UI element
    public float characterFadeDuration = 2.0f;

    void Start()
    {
        SetState(IntroState.Initialize);
    }

    private void SetState(IntroState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case IntroState.Initialize:
                // Hide standard UI
                dialogueUI.SetActive(false);
                perkSelectionUI.SetActive(false);
                statAllocationUI.SetActive(false);

                // Move directly to playing the animation
                SetState(IntroState.PlayOpeningAnimation);
                break;

            case IntroState.PlayOpeningAnimation:
                if (introAnimator != null)
                {
                    introAnimator.gameObject.SetActive(true);
                    introAnimator.SetTrigger(introAnimationTrigger);
                }

                // Start the timer to wait for the animation to finish playing
                StartCoroutine(WaitAndFinishOpeningAnimation());
                break;

            case IntroState.FadeCharacterIn:
                Debug.Log("Fading Character In...");
                StartCoroutine(FadeInCharacterRoutine());
                break;

            case IntroState.PanToGodess:
                Debug.Log("Panning to Goddess...");
                cameraPanner.PanToGoddess();
                break;

            case IntroState.GoddessDialogue1:
                dialogueUI.SetActive(true);
                // dialogueController.StartPhase1();
                Debug.Log("Goddess: 'You are asleep...'");
                break;

            case IntroState.PerkSelection:
                dialogueUI.SetActive(false);
                perkSelectionUI.SetActive(true);
                Debug.Log("Waiting for player to select a perk or choose random...");
                break;

            case IntroState.GoddessDialogue2:
                perkSelectionUI.SetActive(false);
                dialogueUI.SetActive(true);
                // dialogueController.StartPhase2();
                Debug.Log("Goddess: 'An interesting choice...'");
                break;

            case IntroState.PanToStatAllocation:
                dialogueUI.SetActive(false);
                Debug.Log("Panning To Stat Allocation...");
                cameraPanner.PanToStatMenu(); // Trigger the camera
                break;

            case IntroState.StatAllocation:
                perkSelectionUI.SetActive(false);
                statAllocationUI.SetActive(true);
                Debug.Log("Waiting for player to allocate stats...");
                break;

            case IntroState.GoddessDialogue3:
                statAllocationUI.SetActive(false);
                dialogueUI.SetActive(true);
                // dialogueController.StartPhase3();
                Debug.Log("Goddess: 'Your soul is prepared...'");
                break;

            case IntroState.PanToPortal:
                dialogueUI.SetActive(false);
                Debug.Log("Panning To Portal...");
                cameraPanner.PanToPortal(); // Trigger the camera
                break;

            case IntroState.FinalizeAndLoad:
                statAllocationUI.SetActive(false);
                FinalizeCharacterAndTransition();
                break;
        }
    }

    // --- Coroutines ---

    private IEnumerator WaitAndFinishOpeningAnimation()
    {
        yield return new WaitForSeconds(openingAnimationDuration);

        float elapsedTime = 0f;

        // Target the UI Image directly instead of a Canvas Group
        UnityEngine.UI.Image uiImage = introAnimator.GetComponent<UnityEngine.UI.Image>();
        SpriteRenderer spriteRenderer = introAnimator.GetComponent<SpriteRenderer>();

        while (elapsedTime < animationFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / animationFadeOutDuration);

            if (uiImage != null)
            {
                Color c = uiImage.color;
                c.a = alpha;
                uiImage.color = c;
            }
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = alpha;
                spriteRenderer.color = c;
            }

            yield return null;
        }

        introAnimator.gameObject.SetActive(false);
        SetState(IntroState.FadeCharacterIn);
    }

    // --- Public Methods to be called by other scripts or UI Buttons ---

    public void AdvanceFromDialogue()
    {
        if (currentState == IntroState.GoddessDialogue1)
        {
            SetState(IntroState.PerkSelection);
        }
        else if (currentState == IntroState.GoddessDialogue2)
        {
            SetState(IntroState.PanToStatAllocation);
        }
        else if (currentState == IntroState.GoddessDialogue3)
        {
            SetState(IntroState.PanToPortal);
        }
    }

    public void OnPerkConfirmed(bool isRandom)
    {
        if (currentState == IntroState.PerkSelection)
        {
            SetState(IntroState.GoddessDialogue2);
        }
    }

    private IEnumerator FadeInCharacterRoutine()
    {
        if (playerSprite != null)
        {
            float elapsedTime = 0f;
            Color c = playerSprite.color;
            c.a = 0f; // Ensure it starts invisible
            playerSprite.color = c;

            while (elapsedTime < characterFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                c.a = Mathf.Clamp01(elapsedTime / characterFadeDuration);
                playerSprite.color = c;
                yield return null;
            }

            c.a = 1f;
            playerSprite.color = c;
        }
        else
        {
            Debug.LogWarning("No player sprite assigned to fade in!");
        }

        // Wait a brief moment after fading before panning
        yield return new WaitForSeconds(0.5f);

        SetState(IntroState.PanToGodess);
    }

    public void OnGoddessPanComplete()
    {
        if (currentState == IntroState.PanToGodess)
        {
            SetState(IntroState.GoddessDialogue1);
        }
    }

    public void PanCamera()
    {
        if (currentState == IntroState.PanToStatAllocation)
        {
            SetState(IntroState.StatAllocation);
        }

    }


    public void OnStatsConfirmed()
    {
        if (currentState == IntroState.StatAllocation)
        {
            SetState(IntroState.GoddessDialogue3);
        }
    }

    public void OnPortalPanComplete()
    {
        if (currentState == IntroState.PanToPortal)
        {
            SetState(IntroState.FinalizeAndLoad);
        }
    }

    // --- Finalization ---

    private void FinalizeCharacterAndTransition()
    {
        CharacterCreationManager creationData = GetComponent<CharacterCreationManager>();
        PlayerStats.Instance.InitializeFromCreation(creationData);

        Debug.Log("Saving Base Stats and Multipliers...");
        Debug.Log("Establishing Checkpoint 0...");

        SaveLoadManager.Instance.SaveCheckpoint();
        TimeManager.Instance.isTimePaused = false;
    }
}