using Controllers;
using Generics;
using Input;
using PlayerSystems;
using UnityEngine;

namespace Weapons {
    public class GravityGun : EquippableWeapon {
        private const float GRAVITATIONAL_CONSTANT = 0.667408f;
        private HumanoidLandInput input;
        private float holdingRange = 3f;
        private float lerpSpeed = 5f;
        private Transform attackSource;
        private Transform holdPoint;
        private Camera playerCamera;
        private RaycastHit hit;
        private bool gravityMode = true;
        public string modeText;
        private bool alreadySwitchedMode;
        private float range = 50f;
        private float distanceToRb;
        private Grabbable grabbable;
        private Rigidbody rb;

        private void Start() {
            attackSource = PlayerManager.instance.gunAttackSource;
            playerCamera = CameraController.instance.mainCamera;
            input = HumanoidLandInput.instance;
            holdPoint = PlayerManager.instance.GetGrabbableHoldPoint();
            SetModeText();
        }

        private void Update() {
            if (!input.altFireInput && alreadySwitchedMode) {
                alreadySwitchedMode = false;
            }
            Attract();
        }

        public override void Fire() {
            var ray = playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            Physics.Raycast(ray, out hit, range);
            if (hit.collider) {
                hit.transform.TryGetComponent(out grabbable);
                hit.transform.TryGetComponent(out rb);
            }
        }

        public override void AltFire() {
            if (!alreadySwitchedMode) {
                gravityMode = !gravityMode;
                alreadySwitchedMode = true;
                SetModeText();
            }
        }

        public override string GetName() {
            return "Gravity Gun";
        }

        public override bool HasAlternateMode() {
            return true;
        }

        public override string GetModeText() {
            return modeText;
        }

        private void SetModeText() {
            modeText = gravityMode ? "Pull" : "Push";
        }

        private void Attract() {
            if (!grabbable || !rb) {
                return;
            }
            var direction = attackSource.position - rb.position;
            var distance = direction.magnitude;
            if (distance == 0f) {
                return;
            }
            var forceMagnitude = GRAVITATIONAL_CONSTANT * (750f * rb.mass) / distance;
            var force = direction.normalized * forceMagnitude;
            // todo, when object is in front of the player, stop attracting and keep the object hovering
            distanceToRb = Vector3.Distance(rb.position, attackSource.position);
            if (ShouldRepel()) {
                rb.AddForce(-force, ForceMode.Acceleration);
            } else if (ShouldAttract()) {
                rb.AddForce(force, ForceMode.Acceleration);
            } else if (ShouldHold()) {
                // hold in place in hold point
                grabbable.Grab(holdPoint);
            } else {
                grabbable.Drop();
                rb = null;
                grabbable = null;
            }
            // if (!gravityMode && distanceToRb > holdingRange && distanceToRb < range) {
            //     rb.AddForce(-force, ForceMode.Acceleration);
            // } else if (distanceToRb > holdingRange && distanceToRb < range) {
            //     rb.AddForce(force, ForceMode.Acceleration);
            // } else if (distanceToRb < holdingRange) {
            //     rb.velocity = Vector3.zero;
            //     rb.angularVelocity = Vector3.zero;
            //     var pos = Vector3.Lerp(transform.position, attackSource.position, Time.deltaTime * lerpSpeed);
            //     rb.MovePosition(pos);
            // }
        }

        private bool ShouldRepel() {
            return input.fireInput && !gravityMode && distanceToRb < range;
        }

        private bool ShouldAttract() {
            return input.fireInput && distanceToRb > holdingRange && distanceToRb < range;
        }
        
        private bool ShouldHold() {
            return input.fireInput && distanceToRb < holdingRange;
        }
    }
}