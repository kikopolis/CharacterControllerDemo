using UnityEngine;
using UnityEngine.InputSystem;

namespace Input {
    public class HumanoidLandInput : MonoBehaviour {
        public Vector2 move { get; private set; } = Vector2.zero;
        public Vector2 look { get; private set; } = Vector2.zero;
        public bool moveIsPressed { get; private set; }
        public bool invertMouse { get; private set; } = true;
        public bool changeCameraWasPressedThisFrame { get; private set; }
        public float cameraDistance { get; private set; }
        public bool invertCameraDistanceScroll { get; private set; } = true;
        public bool run { get; private set; }
        public bool jump { get; private set; }

        private InputActions inputActions;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction cameraDistanceAction;
        private InputAction runAction;
        private InputAction jumpAction;

        private void Awake() {
            inputActions = new InputActions();
            moveAction = inputActions.HumanoidLand.Move;
            lookAction = inputActions.HumanoidLand.Look;
            cameraDistanceAction = inputActions.HumanoidLand.CameraDistance;
            runAction = inputActions.HumanoidLand.Run;
            jumpAction = inputActions.HumanoidLand.Jump;
        }

        private void OnEnable() {
            inputActions.Enable();

            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;

            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;

            cameraDistanceAction.started += SetCameraDistance;
            cameraDistanceAction.canceled += SetCameraDistance;

            runAction.started += OnRun;
            runAction.canceled += OnRun;
            
            jumpAction.started += OnJump;
            jumpAction.canceled += OnJump;
        }

        private void OnDisable() {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;

            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;

            cameraDistanceAction.started -= SetCameraDistance;
            cameraDistanceAction.canceled -= SetCameraDistance;

            runAction.started -= OnRun;
            runAction.canceled -= OnRun;
            
            jumpAction.started += OnJump;
            jumpAction.canceled += OnJump;

            inputActions.Disable();
        }

        private void Update() {
            changeCameraWasPressedThisFrame = inputActions.HumanoidLand.ChangeCamera.WasPressedThisFrame();
        }

        private void OnMove(InputAction.CallbackContext ctx) {
            move = ctx.ReadValue<Vector2>();
            moveIsPressed = move != Vector2.zero;
        }

        private void OnLook(InputAction.CallbackContext ctx) {
            look = ctx.ReadValue<Vector2>();
        }

        private void SetCameraDistance(InputAction.CallbackContext ctx) {
            cameraDistance = ctx.ReadValue<float>();
        }

        private void OnRun(InputAction.CallbackContext ctx) {
            run = ctx.started;
        }
        
        private void OnJump(InputAction.CallbackContext ctx) {
            jump = ctx.started;
        }
    }
}