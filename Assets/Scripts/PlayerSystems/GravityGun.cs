using System.Collections.Generic;
using Input;
using UnityEngine;

namespace PlayerSystems {
    public class GravityGun : MonoBehaviour, IEquippableWeapon {
        private const float GRAVITATIONAL_CONSTANT = 0.667408f;
        private bool gravityMode;
        private List<Rigidbody> attractees = new();

        public void Fire() {
            // todo make single target only
            foreach (var rb in attractees) {
                if (rb != this) {
                    Attract(rb);
                }
            }
        }

        public void AltFire() {
            gravityMode = !gravityMode;
        }

        private void Attract(Rigidbody rb) {
            var direction = transform.position - rb.position;
            var distance = direction.magnitude;
            if (distance == 0f) {
                return;
            }
            var forceMagnitude = GRAVITATIONAL_CONSTANT * (750f * rb.mass) / distance;
            var force = direction.normalized * forceMagnitude;
            if (gravityMode) {
                rb.AddForce(-force);
            } else {
                rb.AddForce(force);
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!(other.attachedRigidbody == null) || !(other.attachedRigidbody.isKinematic)) {
                if (!(attractees.Contains(other.attachedRigidbody))) {
                    attractees.Add(other.attachedRigidbody);
                }
            }
        }

        private void OnTriggerStay(Collider other) {
            if (attractees.Contains(other.attachedRigidbody)) {
                // if (input.fireInput) {
                //     other.attachedRigidbody.useGravity = false;
                // } else {
                if (!other.gameObject.CompareTag("Player")) {
                    other.attachedRigidbody.useGravity = true;
                }
                // }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (!(other.attachedRigidbody == null)) {
                if (attractees.Contains(other.attachedRigidbody)) {
                    attractees.Remove(other.attachedRigidbody);
                    if (!(other.gameObject.CompareTag("Player"))) {
                        other.attachedRigidbody.useGravity = true;
                    }
                }
            }
        }

        // private void OnCollisionEnter(Collision collision) {
        //     if (input.fireInput) {
        //         if (attractees.Contains(collision.rigidbody)) {
        //             if (!collision.gameObject.CompareTag("Player")) {
        //                 attractees.Remove(collision.rigidbody);
        //                 Destroy(collision.gameObject);
        //             }
        //         }
        //     }
        // }
    }
}