using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;

    public float lookSpeed = 2f; // Sensitivity of swipe rotation
    public float lookXLimit = 45f;

    public Joystick movementJoystick; // Joystick for movement
    public RectTransform touchField; // Touch field for camera control

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0f;
    private bool isTouching = false;
    private Vector2 lastTouchPosition;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleCameraRotation();

        // Apply gravity when not grounded
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleMovement()
    {
        // Get joystick input for movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = walkSpeed * movementJoystick.Vertical;
        float curSpeedY = walkSpeed * movementJoystick.Horizontal;

        // Preserve vertical movement (y-component)
        float verticalVelocity = moveDirection.y;

        // Calculate horizontal movement
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        moveDirection.y = verticalVelocity; // Restore vertical movement
    }

    private void HandleCameraRotation()
    {
        // Handle touch input for camera rotation
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch is within the touch field
            if (RectTransformUtility.RectangleContainsScreenPoint(touchField, touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    isTouching = true;
                    lastTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && isTouching)
                {
                    Vector2 touchDelta = touch.position - lastTouchPosition;
                    lastTouchPosition = touch.position;

                    // Apply the touch delta to camera rotation
                    rotationX -= touchDelta.y * lookSpeed * Time.deltaTime;
                    rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                    float rotationY = touchDelta.x * lookSpeed * Time.deltaTime;

                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                    transform.rotation *= Quaternion.Euler(0, rotationY, 0);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isTouching = false;
                }
            }
        }
    }

    public void JumpButton()
    {
        if (characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
    }
}
