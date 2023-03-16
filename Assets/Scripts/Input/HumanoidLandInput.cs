using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class HumanoidLandInput : MonoBehaviour {
        public static HumanoidLandInput instance { get; private set; }
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
        public bool interactInput { get; private set; }
        public float interactInputTime { get; private set; }
        public bool fireInput { get; private set; }
        public bool altFireInput { get; private set; }
        public bool reloadInput { get; private set; }
        public bool switchCharacterWasPressedThisFrame { get; private set; }
        public bool hotbarOneInput { get; private set; }

        private InputActions inputActions;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction cameraDistanceAction;
        private InputAction runAction;
        private InputAction jumpAction;
        private InputAction crouchAction;
        private InputAction interactAction;
        private InputAction fireAction;
        private InputAction altFireAction;
        private InputAction reloadAction;
        private InputAction hotbarOneAction;

        private void Awake() {
            instance = this;
            inputActions = new InputActions();
            moveAction = inputActions.HumanoidLand.Move;
            lookAction = inputActions.HumanoidLand.Look;
            cameraDistanceAction = inputActions.HumanoidLand.CameraDistance;
            runAction = inputActions.HumanoidLand.Run;
            jumpAction = inputActions.HumanoidLand.Jump;
            crouchAction = inputActions.HumanoidLand.Crouch;
            interactAction = inputActions.HumanoidLand.Interact;
            fireAction = inputActions.HumanoidLand.Fire;
            altFireAction = inputActions.HumanoidLand.AltFire;
            reloadAction = inputActions.HumanoidLand.Reload;
            hotbarOneAction = inputActions.HumanoidLand.Hotbar1;
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

            interactAction.performed += SetInteract;
            interactAction.canceled += SetInteract;

            fireAction.started += SetFire;
            fireAction.canceled += SetFire;

            altFireAction.started += SetAltFire;
            altFireAction.canceled += SetAltFire;

            reloadAction.started += SetReload;
            reloadAction.canceled += SetReload;

            hotbarOneAction.started += SetHotbarOne;
            hotbarOneAction.canceled += SetHotbarOne;
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

            interactAction.performed -= SetInteract;
            interactAction.canceled -= SetInteract;

            fireAction.started -= SetFire;
            fireAction.canceled -= SetFire;

            altFireAction.started -= SetAltFire;
            altFireAction.canceled -= SetAltFire;

            reloadAction.started -= SetReload;
            reloadAction.canceled -= SetReload;

            hotbarOneAction.started -= SetHotbarOne;
            hotbarOneAction.canceled -= SetHotbarOne;

            inputActions.Disable();
        }

        private void Update() {
            changeCameraWasPressedThisFrame = inputActions.HumanoidLand.ChangeCamera.WasPressedThisFrame();
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

        private void SetInteract(InputAction.CallbackContext ctx) {
            interactInput = ctx.started;
            interactInputTime = ctx.ReadValue<float>();
        }

        private void SetFire(InputAction.CallbackContext ctx) {
            fireInput = ctx.started;
        }

        private void SetAltFire(InputAction.CallbackContext ctx) {
            altFireInput = ctx.started;
        }

        private void SetReload(InputAction.CallbackContext ctx) {
            reloadInput = ctx.started;
        }

        private void SetHotbarOne(InputAction.CallbackContext ctx) {
            hotbarOneInput = ctx.started;
        }
    }
}