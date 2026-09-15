using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Input Bindings")]
    [Tooltip("Drag your Move InputActionReference here from the Project window")]
    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // The action must be enabled to listen for hardware input
        if (moveAction != null)
        {
            moveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Prevent memory leaks by disabling when the character is destroyed/inactive
        if (moveAction != null)
        {
            moveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (moveAction == null) return;

        // Read the joystick/keyboard vector directly from the reference
        movementInput = moveAction.action.ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            animator.SetFloat("InputX", movementInput.x);
            animator.SetFloat("InputY", movementInput.y);

            animator.SetFloat("LastInputX", movementInput.x);
            animator.SetFloat("LastInputY", movementInput.y);
        }

        animator.SetBool("IsMoving", movementInput.magnitude > 0);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementInput * (moveSpeed * Time.fixedDeltaTime));
    }
}