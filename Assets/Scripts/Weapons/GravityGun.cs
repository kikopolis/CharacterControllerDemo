using System;
using Controllers;
using Input;
using PlayerSystems;
using UnityEngine;

namespace Weapons {
    public class GravityGun : MonoBehaviour, IEquippableWeapon {
        private const float GRAVITATIONAL_CONSTANT = 0.667408f;
        private HumanoidLandInput input;
        private Transform attackSource;
        private Camera playerCamera;
        private RaycastHit hit;
        private bool gravityMode;
        private bool alreadySwitchedMode;
        private float range = 50f;

        private void Start() {
            attackSource = PlayerManager.instance.gunAttackSource;
            playerCamera = CameraController.instance.mainCamera;
            input = HumanoidLandInput.instance;
        }

        private void Update() {
            if (!input.altFireInput && alreadySwitchedMode) {
                alreadySwitchedMode = false;
            }
        }

        public void Fire() {
            var ray = playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            Physics.Raycast(ray, out hit, range);
            if (hit.collider.attachedRigidbody != null) {
                Attract(hit.collider.attachedRigidbody);
            }
        }

        public void AltFire() {
            if (!alreadySwitchedMode) {
                gravityMode = !gravityMode;
                alreadySwitchedMode = true;
            }
        }

        private void Attract(Rigidbody rb) {
            var direction = attackSource.position - rb.position;
            var distance = direction.magnitude;
            if (distance == 0f) {
                return;
            }
            var forceMagnitude = GRAVITATIONAL_CONSTANT * (750f * rb.mass) / distance;
            var force = direction.normalized * forceMagnitude;
            // todo, when object is in front of the player, stop attracting and keep the object hovering
            var distanceToRb = Vector3.Distance(rb.position, attackSource.position);
            if (gravityMode && distanceToRb > 1f && distanceToRb < range) {
                rb.AddForce(-force, ForceMode.Acceleration);
            } else if (distanceToRb > 1f && distanceToRb < range) {
                rb.AddForce(force, ForceMode.Acceleration);
            } else if (distanceToRb < 1f) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}