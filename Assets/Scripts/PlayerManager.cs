﻿using Cinemachine;
using Controllers;
using Input;
using PlayerSystems;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public static PlayerManager instance { get; private set; }
    [ SerializeField ]
    private HumanoidLandInput input;
    [ SerializeField ]
    private CameraController cameraController;
    [ SerializeField ]
    private HumanoidLandController redController;
    // [ SerializeField ]
    // private HumanoidLandController greenController;
    [ SerializeField ]
    private Inventory redInventory;
    private WeaponSystem weaponSystem;
    [SerializeField]
    private Transform equipmentAnchor;
    [SerializeField]
    private Transform grabbableAnchor;
    private IEquipableOnHotbar[] hotbar = new IEquipableOnHotbar[10];
    private IEquipableOnHotbar currentEquippedWeapon;
    private IngameUiManager uiManager;

    public Transform GetGrabbableAnchor() {
        return grabbableAnchor;
    }

    private void Awake() {
        instance = this;
        weaponSystem = GetComponent<WeaponSystem>();
        redController.enabled = true;
        // greenController.enabled = false;
    }

    private void Start() {
        uiManager = IngameUiManager.instance;
    }

    private void FixedUpdate() {
        // todo testing equipment generation
        GenerateTestHotbar();

        SelectWeapon();
        Fire();
        if (input.switchCharacterWasPressedThisFrame) {
            SwitchCharacter();
        }
    }
    
    private void Fire() {
        if (currentEquippedWeapon == null) {
            return;
        }
        if (input.fireInput) {
            currentEquippedWeapon.Fire();
        } else if (input.altFireInput) {
            currentEquippedWeapon.AltFire();
        }
    }

    private void GenerateTestHotbar() {
        hotbar[0] = weaponSystem.GetGravityGun();
    }

    private void SelectWeapon() {
        if (input.hotbarOneInput) {
            currentEquippedWeapon = hotbar[0];
            uiManager.SelectWeapon(currentEquippedWeapon);
        }
    }

    private void SwitchCharacter() {
        // todo implement game systems and then come back to this
        return;
        redController.enabled = !redController.enabled;
        // greenController.enabled = !greenController.enabled;

        // if (redController.enabled && greenController.enabled) {
        //     throw new MoreThanOneCharacterEnabledException();
        // }

        var currentCameraFollow = GetCurrentCameraFollow();
        var thirdPersonCamera = cameraController.thirdPersonCamera.GetComponent<CinemachineVirtualCamera>();
        thirdPersonCamera.Follow = currentCameraFollow;
        thirdPersonCamera.LookAt = currentCameraFollow;

        cameraController.firstPersonCamera.GetComponent<CinemachineVirtualCamera>().Follow = currentCameraFollow;

        var orbitalCamera = cameraController.orbitalCamera.GetComponent<CinemachineVirtualCamera>();
        orbitalCamera.Follow = currentCameraFollow;
        orbitalCamera.LookAt = currentCameraFollow;
    }
    
    public Transform GetEquipmentAnchor() {
        return equipmentAnchor;
    }

    public Transform GetCurrentCameraFollow() {
        return redController.cameraFollow;
        // return redController.enabled ? redController.cameraFollow : greenController.cameraFollow;
    }

    public void AddToHotbar(IEquipableOnHotbar item, int slot) {
        if (slot > 10) {
            return;
        }
        hotbar[slot] = item;
    }

    public void RemoveFromHotbar(IEquipableOnHotbar item, int slot) {
        if (slot > 10) {
            return;
        }
        hotbar[slot] = null;
    }
}