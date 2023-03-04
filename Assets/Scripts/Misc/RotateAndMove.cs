using System;
using System.Collections.Generic;
using UnityEngine;

namespace Misc {
    public class RotateAndMove : MonoBehaviour {
        private Rigidbody rigidBody;
        [ SerializeField ]
        private bool rotationEnabled = true;
        [ SerializeField ]
        private float rotationSpeed = 20f;
        [ SerializeField ]
        private bool movementEnabled = true;
        [ SerializeField ]
        private float movementSpeed = 1f;
        private Vector3 startPosition;
        private Vector3 endPosition;
        private Vector3 positionLastFrame;
        private float timescale;
        private Dictionary<Rigidbody, float> rbsOnPlatformInTime = new();
        [ SerializeField ]
        private List<Rigidbody> rbsOnPlatform = new();

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            startPosition = rigidBody.position;
            endPosition = new Vector3(startPosition.x + 3f, startPosition.y + 3f, startPosition.z);
        }

        private void FixedUpdate() {
            if (rotationEnabled) {
                rigidBody.rotation = Quaternion.Euler(rigidBody.rotation.eulerAngles.x,
                                                      rigidBody.rotation.eulerAngles.y
                                                    + rotationSpeed * Time.fixedDeltaTime,
                                                      rigidBody.rotation.eulerAngles.z);
            }
            if (rbsOnPlatform.Count != rbsOnPlatformInTime.Count) {
                rbsOnPlatformInTime.Clear();
                foreach (var rb in rbsOnPlatform) {
                    rbsOnPlatformInTime.Add(rb, 1f);
                }
            }
            if (movementEnabled) {
                positionLastFrame = rigidBody.position;
                timescale = movementSpeed / Vector3.Distance(startPosition, endPosition);
                rigidBody.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(Time.time * timescale % 2 - 1));
            }
            foreach (var rb in rbsOnPlatform) {
                rbsOnPlatformInTime.TryGetValue(rb, out var timer);
                if (timer < 1f) {
                    rbsOnPlatformInTime[rb] += Time.deltaTime * 4f;
                } else if (timer > 1f) {
                    rbsOnPlatformInTime[rb] = 1f;
                }
                RotateAndMoveRbOnPlatform(rb, timer);
            }
        }

        private void RotateAndMoveRbOnPlatform(Rigidbody rb, float timer) {
            if (rotationEnabled) {
                var rotationAmount = rotationSpeed * timer * Time.deltaTime;
                var localAngleAxis = Quaternion.AngleAxis(rotationAmount, rigidBody.transform.up);
                rb.position = (localAngleAxis * (rb.position - rigidBody.position)) + rigidBody.position;
                var globalAngleAxis
                    = Quaternion.AngleAxis(rotationAmount,
                                           rb.transform.InverseTransformDirection(rigidBody.transform.up));
                rb.rotation *= globalAngleAxis;
            }
            if (movementEnabled) {
                rb.position += (rigidBody.position - positionLastFrame) * timer;
            }
        }

        private void OnTriggerEnter(Collider other) {
            if (!(other.attachedRigidbody == null) && !(other.attachedRigidbody.isKinematic)) {
                if (!(rbsOnPlatform.Contains(other.attachedRigidbody))) { 
                    rbsOnPlatform.Add(other.attachedRigidbody);
                    rbsOnPlatformInTime.Add(other.attachedRigidbody, 0f);
                }
            }
        }

        private void OnTriggerExit(Collider other) {
            if (!(other.attachedRigidbody == null)) {
                if (rbsOnPlatform.Contains(other.attachedRigidbody)) {
                    rbsOnPlatform.Remove(other.attachedRigidbody);
                    rbsOnPlatformInTime.Remove(other.attachedRigidbody);
                }
            }
        }
    }
}