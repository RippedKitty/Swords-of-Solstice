using UnityEngine;
using TMPro; // Required for TextMeshPro
using System.Collections;

public class DialogueController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueTextComponent;
    public IntroSequenceManager introManager;

    [Header("Typing Settings")]
    public float typingSpeed = 0.05f;

    [Header("Dialogue Content")]
    [TextArea(2, 4)] public string[] dialoguePhase1; // "You are asleep..."
    [TextArea(2, 4)] public string[] dialoguePhase2; // "An interesting choice..."
    [TextArea(2, 4)] public string[] dialoguePhase3; // "Your path is set..."

    private string[] currentDialogueBlock;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Update()
    {
        // Listen for left mouse click or Spacebar to advance/skip
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // If currently typing, skip the animation and show the full line
                StopCoroutine(typingCoroutine);
                dialogueTextComponent.text = currentDialogueBlock[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // If done typing, move to the next line
                AdvanceLine();
            }
        }
    }

    // --- Public methods called by IntroSequenceManager ---

    public void StartPhase1()
    {
        StartDialogueBlock(dialoguePhase1);
    }

    public void StartPhase2()
    {
        StartDialogueBlock(dialoguePhase2);
    }

    public void StartPhase3()
    {
        StartDialogueBlock(dialoguePhase3);
    }

    // --- Internal Logic ---

    private void StartDialogueBlock(string[] linesToType)
    {
        currentDialogueBlock = linesToType;
        currentLineIndex = 0;

        // Clear text before starting
        dialogueTextComponent.text = "";

        // Start typing the first line
        typingCoroutine = StartCoroutine(TypeLine());
    }

    private void AdvanceLine()
    {
        currentLineIndex++;

        if (currentLineIndex < currentDialogueBlock.Length)
        {
            // Type the next line
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            // End of this dialogue block, tell the Intro Manager to move on
            dialogueTextComponent.text = "";
            introManager.AdvanceFromDialogue();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueTextComponent.text = "";

        // Convert the string to a character array and type one by one
        foreach (char c in currentDialogueBlock[currentLineIndex].ToCharArray())
        {
            dialogueTextComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}