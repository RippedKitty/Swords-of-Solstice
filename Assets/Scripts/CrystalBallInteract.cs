using UnityEngine;
using UnityEngine.InputSystem;

public class CrystalBallInteract : MonoBehaviour
{
    [Header("References")]
    public IntroSequenceManager introManager;
    
    [Header("State")]
    public bool playerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Crystal Ball: Press 'E' or Space to interact.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        // Only allow interaction if the player is in range and the sequence is waiting for it
        if (playerInRange && introManager != null && introManager.currentState == IntroSequenceManager.IntroState.WaitForCrystalBall)
        {
            bool interactPressed = false;
            
            if (Keyboard.current != null)
            {
                interactPressed = Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame;
            }

            if (interactPressed)
            {
                // Player interacted, trigger the stat allocation menu!
                introManager.TriggerStatAllocation();
            }
        }
    }
}
