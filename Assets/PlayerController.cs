using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))] 
public class PlayerController : MonoBehaviour
{
    [Header("Réglages Déplacement")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer; 
    private Vector2 moveInput;
    private Vector2 lastMoveInput = Vector2.down;
    private Interactable currentInteractable; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        if (moveInput != Vector2.zero)
        {
            lastMoveInput = moveInput;
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    public void SetCurrentInteractable(Interactable interactable)
    {
        currentInteractable = interactable;
    }

    private void Update()
    {
        if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        anim.SetFloat("MoveX", moveInput.x);
        anim.SetFloat("MoveY", moveInput.y);
        anim.SetFloat("LastMoveX", lastMoveInput.x);
        anim.SetFloat("LastMoveY", lastMoveInput.y);
        anim.SetFloat("Speed", moveInput.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * (moveSpeed * Time.fixedDeltaTime));
    }
}