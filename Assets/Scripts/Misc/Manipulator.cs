using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Input;
using UnityEngine;

public class Manipulator : MonoBehaviour {
    private const float GRAVITATIONAL_CONSTANT = 0.667408f;
    [ SerializeField ]
    private HumanoidLandInput input;
    [ SerializeField ]
    private OnScreenCounter onScreenCounter;
    [ SerializeField ]
    private bool manipulatorEnabled;
    [ SerializeField ]
    private bool manipulatorToggledOn;
    [ SerializeField ]
    private bool manipulatorModeToggled;
    private MeshRenderer meshRenderer;
    private List<Rigidbody> attractees = new();

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update() {
        // if (input.interactInput != 0) {
        //     OnOff();
        // }
        if (input.altFireInput) {
            ChangeMode();
        }
        manipulatorToggledOn = input.fireInput;
        SetColor();
    }

    private void OnOff() {
        manipulatorEnabled = !manipulatorEnabled;
    }

    private void ChangeMode() {
        manipulatorModeToggled = !manipulatorModeToggled;
    }

    private void SetColor() {
        if (manipulatorEnabled || manipulatorToggledOn) {
            if (manipulatorModeToggled) {
                meshRenderer.material.color = Color.red;
            } else {
                meshRenderer.material.color = Color.blue;
            }
        } else {
            if (manipulatorModeToggled) {
                meshRenderer.material.color = Color.white + Color.red;
            } else {
                meshRenderer.material.color = Color.white + Color.blue;
            }
        }
    }

    private void FixedUpdate() {
        foreach (var rb in attractees) {
            if (rb != this) {
                Attract(rb);
            }
        }
    }

    private void Attract(Rigidbody rb) {
        if (manipulatorEnabled || manipulatorToggledOn) {
            var direction = transform.position - rb.position;
            var distance = direction.magnitude;
            if (distance == 0f) {
                return;
            }
            var forceMagnitude = GRAVITATIONAL_CONSTANT * (750f * rb.mass) / distance;
            var force = direction.normalized * forceMagnitude;
            if (manipulatorModeToggled) {
                rb.AddForce(-force);
            } else {
                rb.AddForce(force);
            }
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
            if (manipulatorEnabled || manipulatorToggledOn) {
                other.attachedRigidbody.useGravity = false;
            } else {
                if (!other.gameObject.CompareTag("Player")) {
                    other.attachedRigidbody.useGravity = true;
                }
            }
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

    private void OnCollisionEnter(Collision collision) {
        if (manipulatorEnabled || manipulatorToggledOn) {
            if (attractees.Contains(collision.rigidbody)) {
                if (!collision.gameObject.CompareTag("Player")) {
                    attractees.Remove(collision.rigidbody);
                    Destroy(collision.gameObject);
                    onScreenCounter.counter++;
                }
            }
        }
    }
}