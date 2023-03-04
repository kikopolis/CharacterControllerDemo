using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class HumanoidLandInput : MonoBehaviour {
        public Vector2 moveInput { get; private set; } = Vector2.zero;
        public Vector2 lookInput { get; private set; } = Vector2.zero;
        public bool moveIsPressed { get; private set; }
        public bool invertMouse { get; private set; } = true;
        public bool changeCameraWasPressedThisFrame { get; private set; }
        public float cameraDistance { get; private set; }
        public bool invertCameraDistanceScroll { get; private set; } = true;
        public bool runInput { get; private set; }
        public bool jumpInput { get; private set; }
        public bool crouchInput { get; private set; }
        public float activateInput { get; private set; }
        public bool onOffWasPressedThisFrame { get; private set; }
        public bool modeWasPressedThisFrame { get; private set; }
        public bool switchCharacterWasPressedThisFrame { get; private set; }

        private InputActions inputActions;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction cameraDistanceAction;
        private InputAction runAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction activateAction;

        private void Awake() {
            inputActions = new InputActions();
            moveAction = inputActions.HumanoidLand.Move;
            lookAction = inputActions.HumanoidLand.Look;
            cameraDistanceAction = inputActions.HumanoidLand.CameraDistance;
            runAction = inputActions.HumanoidLand.Run;
            jumpAction = inputActions.HumanoidLand.Jump;
            crouchAction = inputActions.HumanoidLand.Crouch;
            activateAction = inputActions.HumanoidLand.Activate;
        }

        private void OnEnable() {
            inputActions.Enable();

            moveAction.performed += SetMove;
            moveAction.canceled += SetMove;

            lookAction.performed += SetLook;
            lookAction.canceled += SetLook;

            cameraDistanceAction.started += SetCameraDistance;
            cameraDistanceAction.canceled += SetCameraDistance;

            runAction.started += SetRun;
            runAction.canceled += SetRun;

            jumpAction.started += SetJump;
            jumpAction.canceled += SetJump;

            crouchAction.started += SetCrouch;
            crouchAction.canceled += SetCrouch;

            activateAction.performed += SetActivate;
            activateAction.canceled += SetActivate;
        }

        private void OnDisable() {
            moveAction.performed -= SetMove;
            moveAction.canceled -= SetMove;

            lookAction.performed -= SetLook;
            lookAction.canceled -= SetLook;

            cameraDistanceAction.started -= SetCameraDistance;
            cameraDistanceAction.canceled -= SetCameraDistance;

            runAction.started -= SetRun;
            runAction.canceled -= SetRun;

            jumpAction.started -= SetJump;
            jumpAction.canceled -= SetJump;

            crouchAction.started -= SetCrouch;
            crouchAction.canceled -= SetCrouch;

            activateAction.performed -= SetActivate;
            activateAction.canceled -= SetActivate;

            inputActions.Disable();
        }

        private void Update() {
            changeCameraWasPressedThisFrame = inputActions.HumanoidLand.ChangeCamera.WasPressedThisFrame();
            onOffWasPressedThisFrame = inputActions.HumanoidLand.OnOff.WasPressedThisFrame();
            modeWasPressedThisFrame = inputActions.HumanoidLand.Mode.WasPressedThisFrame();
            switchCharacterWasPressedThisFrame = inputActions.HumanoidLand.SwitchCharacter.WasPressedThisFrame();
        }

        private void SetMove(InputAction.CallbackContext ctx) {
            moveInput = ctx.ReadValue<Vector2>();
            moveIsPressed = moveInput != Vector2.zero;
        }

        private void SetLook(InputAction.CallbackContext ctx) {
            lookInput = ctx.ReadValue<Vector2>();
        }

        private void SetCameraDistance(InputAction.CallbackContext ctx) {
            cameraDistance = ctx.ReadValue<float>();
        }

        private void SetRun(InputAction.CallbackContext ctx) {
            runInput = ctx.started;
        }

        private void SetJump(InputAction.CallbackContext ctx) {
            jumpInput = ctx.started;
        }

        private void SetCrouch(InputAction.CallbackContext ctx) {
            crouchInput = ctx.started;
        }

        private void SetActivate(InputAction.CallbackContext ctx) {
            activateInput = ctx.ReadValue<float>();
        }
    }
}