using UnityEngine;

public class AstronautController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 50f; // The force applied to move
    public float turnSpeed = 20f; // The force applied to rotate
    public float boostMultiplier = 2f; // How much faster the boost is

    private Rigidbody rb; // Reference to the Rigidbody component

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        // This locks the cursor to the center of the screen, which is great for this type of controller
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // FixedUpdate is used for physics calculations
    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Check for boost input (Left Shift)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boostMultiplier : moveSpeed;

        // Forward and backward thrusters (W/S keys)
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector3.forward * currentSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(Vector3.back * currentSpeed);
        }

        // Strafing thrusters (A/D keys)
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(Vector3.left * currentSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeForce(Vector3.right * currentSpeed);
        }

        // Up and down thrusters (Space/Left Ctrl)
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * currentSpeed);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            rb.AddRelativeForce(Vector3.down * currentSpeed);
        }
    }

    void HandleRotation()
    {
        // Get mouse input for rotation
        float yaw = Input.GetAxis("Mouse X") * turnSpeed * Time.fixedDeltaTime;
        float pitch = Input.GetAxis("Mouse Y") * turnSpeed * Time.fixedDeltaTime;

        // Apply rotation as torque. We use AddRelativeTorque to rotate in local space.
        // This makes the controls feel intuitive, like you're in the pilot's seat.
        rb.AddRelativeTorque(Vector3.up * yaw);
        rb.AddRelativeTorque(Vector3.left * pitch); // Using left instead of right inverts the pitch, which often feels more natural
    }
}