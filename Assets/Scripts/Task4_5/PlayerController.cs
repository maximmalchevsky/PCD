using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Task4_5
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public Camera playerCamera;
        public float moveSpeed = 6f;
        public float runSpeed = 10f;
        public float lookSpeed = 2f;
        public float gravity = -19.62f;
        public float jumpHeight = 1.5f;

        private CharacterController controller;
        private Transform cameraTransform;
        private Vector3 velocity;
        private float verticalLookRotation = 0f;
        private bool isGrounded;

        void Start()
        {
            controller = GetComponent<CharacterController>();
            FindCamera();
        }

        private void FindCamera()
        {
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }
            if (playerCamera != null)
            {
                cameraTransform = playerCamera.transform;
            }
        }

        void Update()
        {
            if (controller == null || !controller.enabled) return;

            if (cameraTransform == null || playerCamera == null)
            {
                FindCamera();
                if (cameraTransform == null || playerCamera == null) return;
            }

            bool isFPSActive = playerCamera.enabled && playerCamera.gameObject.activeInHierarchy;
            if (!isFPSActive) return;

            bool escPressed = Input.GetKeyDown(KeyCode.Escape);
            bool clickPressed = Input.GetMouseButtonDown(0);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                escPressed |= Keyboard.current.escapeKey.wasPressedThisFrame;
            }
            if (Mouse.current != null)
            {
                clickPressed |= Mouse.current.leftButton.wasPressedThisFrame;
            }
#endif

            if (escPressed)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            if (clickPressed && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            isGrounded = controller.isGrounded;
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float mouseX = 0f;
            float mouseY = 0f;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                try
                {
                    mouseX = Input.GetAxis("Mouse X") * lookSpeed;
                    mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
                }
                catch { }

#if ENABLE_INPUT_SYSTEM
                if (Mouse.current != null && Mathf.Abs(mouseX) < 0.001f && Mathf.Abs(mouseY) < 0.001f)
                {
                    Vector2 delta = Mouse.current.delta.ReadValue() * 0.1f * lookSpeed;
                    mouseX = delta.x;
                    mouseY = delta.y;
                }
#endif
            }

            transform.Rotate(Vector3.up * mouseX);

            verticalLookRotation -= mouseY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -85f, 85f);
            cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

            float horizontal = 0f;
            float vertical = 0f;

            try
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
            }
            catch { }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) vertical += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) vertical -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontal -= 1f;
            }
#endif

            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            vertical = Mathf.Clamp(vertical, -1f, 1f);

            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                isRunning |= Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
            }
#endif
            float currentSpeed = isRunning ? runSpeed : moveSpeed;

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            if (move.sqrMagnitude > 1f) move.Normalize();
            controller.Move(move * currentSpeed * Time.deltaTime);

            bool jumpPressed = false;
            try
            {
                jumpPressed = Input.GetButtonDown("Jump");
            }
            catch { }

            jumpPressed |= Input.GetKeyDown(KeyCode.Space);
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                jumpPressed |= Keyboard.current.spaceKey.wasPressedThisFrame;
            }
#endif

            if (jumpPressed && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
