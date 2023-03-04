using System;
using System.Collections.Generic;
using Cinemachine;
using Controllers;
using Input;
using OpenCover.Framework.Model;
using PlayerSystems;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
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
    [ SerializeField ]
    private WeaponSystem weaponSystem;
    [ SerializeField ]
    private IEquippableWeapon[] hotbar = new IEquippableWeapon[10];
    private IEquippableWeapon currentEquippedWeapon;

    private void Awake() {
        redController.enabled = true;
        // greenController.enabled = false;
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

    private void GenerateTestHotbar() {
        hotbar[0] = weaponSystem.gravityGun;
    }

    private void SelectWeapon() {
        if (input.hotbarOneInput) {
            currentEquippedWeapon = hotbar[0];
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

    private Transform GetCurrentCameraFollow() {
        return redController.cameraFollow;
        // return redController.enabled ? redController.cameraFollow : greenController.cameraFollow;
    }

    public void AddEquippable(IEquippableWeapon item, int slot) {
        if (slot > 10) {
            return;
        }
        hotbar[slot] = item;
    }

    public void RemoveEquippable(IEquippableWeapon item, int slot) {
        if (slot > 10) {
            return;
        }
        hotbar[slot] = null;
    }
}