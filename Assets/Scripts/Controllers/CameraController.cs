using Cinemachine;
using Input;
using UnityEngine;

namespace Controllers {
    public class CameraController : MonoBehaviour {
        public static CameraController instance { get; private set; }
        private const int ACTIVE_CAMERA_PRIORITY_MODIFIER = 31337;
        public bool usingOrbitalCamera { get; private set; }
        public bool usingFirstPersonCamera { get; private set; }
        public bool usingThirdPersonCamera { get; private set; }
        [ SerializeField ]
        private HumanoidLandInput input;
        [ SerializeField ]
        private float minCameraDistance = 2f;
        [ SerializeField ]
        private float maxCameraDistance = 16f;
        [ SerializeField ]
        private float minOrbitalCameraDistance = 2f;
        [ SerializeField ]
        private float maxOrbitalCameraDistance = 36f;

        public CinemachineVirtualCamera activeCamera { get; private set; }
        private CinemachineFramingTransposer thirdPersonCameraTransposer;
        private CinemachineFramingTransposer orbitalCameraTransposer;

        public Camera mainCamera;
        public CinemachineVirtualCamera orbitalCamera;
        public CinemachineVirtualCamera thirdPersonCamera;
        public CinemachineVirtualCamera firstPersonCamera;

        private void Awake() {
            instance = this;
            thirdPersonCameraTransposer = thirdPersonCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            orbitalCameraTransposer = orbitalCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        private void Start() {
            SetDefaultCamera();
        }

        private void Update() {
            if (input.cameraDistance != 0) {
                SetCameraDistance();
            }
            if (input.changeCameraWasPressedThisFrame) {
                ChangeCamera();
            }
        }

        private void ChangeCamera() {
            if (thirdPersonCamera == activeCamera) {
                SetCameraPriorities(thirdPersonCamera, firstPersonCamera);
                usingFirstPersonCamera = true;
                usingThirdPersonCamera = false;
                usingOrbitalCamera = false;
                // hides the player if first person camera is active
                // mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player (Self)"));
            } else if (firstPersonCamera == activeCamera) {
                SetCameraPriorities(firstPersonCamera, orbitalCamera);
                usingFirstPersonCamera = false;
                usingThirdPersonCamera = false;
                usingOrbitalCamera = true;
                // shows the player if first person camera is active
                // mainCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player (Self)");
            } else if (orbitalCamera == activeCamera) {
                SetCameraPriorities(orbitalCamera, thirdPersonCamera);
                usingFirstPersonCamera = false;
                usingThirdPersonCamera = true;
                usingOrbitalCamera = false;
            }
        }

        private void SetDefaultCamera() {
            firstPersonCamera.Priority += ACTIVE_CAMERA_PRIORITY_MODIFIER;
            activeCamera = firstPersonCamera;
            usingFirstPersonCamera = true;
            usingThirdPersonCamera = false;
            usingOrbitalCamera = false;
        }

        private void SetCameraPriorities(CinemachineVirtualCamera from, CinemachineVirtualCamera to) {
            from.Priority -= ACTIVE_CAMERA_PRIORITY_MODIFIER;
            to.Priority += ACTIVE_CAMERA_PRIORITY_MODIFIER;
            activeCamera = to;
        }

        private void SetCameraDistance() {
            if (activeCamera == thirdPersonCamera) {
                thirdPersonCameraTransposer.m_CameraDistance
                    += input.invertCameraDistanceScroll ? input.cameraDistance : -input.cameraDistance;
                thirdPersonCameraTransposer.m_CameraDistance
                    = Mathf.Clamp(thirdPersonCameraTransposer.m_CameraDistance,
                                  minCameraDistance,
                                  maxCameraDistance);
            } else if (activeCamera == orbitalCamera) {
                orbitalCameraTransposer.m_CameraDistance
                    += input.invertCameraDistanceScroll ? input.cameraDistance : -input.cameraDistance;
                orbitalCameraTransposer.m_CameraDistance
                    = Mathf.Clamp(orbitalCameraTransposer.m_CameraDistance,
                                  minOrbitalCameraDistance,
                                  maxOrbitalCameraDistance);
            }
        }
    }
}