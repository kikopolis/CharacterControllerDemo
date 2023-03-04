using System;
using Cinemachine;
using Controllers;
using Exception;
using Input;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [ SerializeField ]
    private HumanoidLandInput input;
    [ SerializeField ]
    private CameraController cameraController;
    [ SerializeField ]
    private HumanoidLandController redController;
    [ SerializeField ]
    private HumanoidLandController greenController;

    private bool vsyncEnabled;
    private int targetFramerate = 60;
    private float targetPhysicsRate = 60f;

    private void Awake() {
        QualitySettings.vSyncCount = vsyncEnabled ? 1 : 0;
        Application.targetFrameRate = targetFramerate;
        Time.fixedDeltaTime = 1f / targetPhysicsRate;
        Time.maximumDeltaTime = Time.fixedDeltaTime * 1.25f;

        redController.enabled = true;
        redController.GetComponentInChildren<Manipulator>().enabled = true;
        greenController.enabled = false;
        greenController.GetComponentInChildren<Manipulator>().enabled = false;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (!input.switchCharacterWasPressedThisFrame) {
            return;
        }

        redController.enabled = !redController.enabled;
        greenController.enabled = !greenController.enabled;

        if (redController.enabled && greenController.enabled) {
            throw new MoreThanOneCharacterEnabledException();
        }

        var currentCameraFollow = GetCurrentCameraFollow();
        var thirdPersonCamera = cameraController.thirdPersonCamera.GetComponent<CinemachineVirtualCamera>();
        thirdPersonCamera.Follow = currentCameraFollow;
        thirdPersonCamera.LookAt = currentCameraFollow;

        cameraController.firstPersonCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentCameraFollow;

        var orbitalCamera = cameraController.orbitalCamera.GetComponent<CinemachineVirtualCamera>();
        orbitalCamera.Follow = currentCameraFollow;
        orbitalCamera.LookAt = currentCameraFollow;

        var redManipulator = redController.GetComponentInChildren<Manipulator>();
        redManipulator.enabled = !redManipulator.enabled;
        var greenManipulator = greenController.GetComponentInChildren<Manipulator>();
        greenManipulator.enabled = !greenManipulator.enabled;
    }

    private Transform GetCurrentCameraFollow() {
        return redController.enabled ? redController.cameraFollow : greenController.cameraFollow;
    }
}