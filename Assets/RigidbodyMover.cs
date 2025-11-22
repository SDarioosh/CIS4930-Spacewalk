using UnityEngine;
using UnityEngine.InputSystem;

// This makes sure the script is on an object that has a Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMover : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast the player moves horizontally (W, A, S, D).")]
    public float moveSpeed = 3.0f;

    [Tooltip("How fast the player moves vertically in zero-g (E, Q).")]
    public float flySpeed = 3.0f; // <-- NEW VARIABLE

    [Header("Dependencies")]
    [Tooltip("The camera used to determine 'forward' (usually your Main Camera).")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector2 moveInput; // For WASD
    private float flyInput;    // For E/Q

    void Start()
    {
        // Get the Rigidbody component on this object
        rb = GetComponent<Rigidbody>();

        // Make sure we have a camera assigned
        if (cameraTransform == null)
        {
            Debug.LogError("RigidbodyMover needs a 'Camera Transform' assigned! Please drag your 'Main Camera' into the slot.", this);
            this.enabled = false; // Disable the script
        }
    }

    void Update()
    {
        // --- Read WASD input ---
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y = 1;
        else if (Keyboard.current.sKey.isPressed) moveInput.y = -1;

        if (Keyboard.current.dKey.isPressed) moveInput.x = 1;
        else if (Keyboard.current.aKey.isPressed) moveInput.x = -1;

        // --- Read E/Q (fly up/down) input ---
        flyInput = 0;
        if (Keyboard.current.eKey.isPressed) // E = Up
            flyInput = 1;
        else if (Keyboard.current.qKey.isPressed) // Q = Down
            flyInput = -1;
    }

    void FixedUpdate()
    {
        // --- 1. Get Camera's Forward/Right directions ---
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // --- 2. Calculate horizontal velocity (from WASD) ---
        Vector3 horizontalVelocity = (camForward * moveInput.y + camRight * moveInput.x).normalized * moveSpeed;

        // --- 3. Check if we are in space (zero gravity) ---
        // This is the "switch" for our two movement modes.
        bool inSpace = Physics.gravity.y == 0f;

        if (inSpace)
        {
            // --- ZERO-G (SPACE) LOGIC ---
            // We control all three axes.

            // Calculate vertical velocity from E/Q keys
            float yVelocity = flyInput * flySpeed;

            rb.linearVelocity = new Vector3(
                horizontalVelocity.x,
                yVelocity, // Use our calculated fly speed
                horizontalVelocity.z
            );
        }
        else
        {
            // --- PLANETARY (GRAVITY) LOGIC ---
            // We only control X and Z. Y is handled by gravity.

            rb.linearVelocity = new Vector3(
                horizontalVelocity.x,
                rb.linearVelocity.y, // Preserve existing Y velocity (so gravity works)
                horizontalVelocity.z
            );
        }
    }
}