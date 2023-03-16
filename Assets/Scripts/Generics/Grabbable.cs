using UnityEngine;

namespace Generics {
    public class Grabbable : MonoBehaviour {
        private Rigidbody rb;
        private Transform holdPoint;
        private float lerpSpeed = 10f;

        public void Grab(Transform point) {
            holdPoint = point;
            rb.useGravity = false;
            if (rb.interpolation != RigidbodyInterpolation.Interpolate) {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        public void Drop() {
            rb.useGravity = true;
            holdPoint = null;
        }

        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            if (holdPoint != null && rb != null) {
                // Reset velocity to fix object jittering when player moves
                rb.velocity = Vector3.zero;
                var pos = Vector3.Lerp(transform.position, holdPoint.position, Time.deltaTime * lerpSpeed);
                rb.MovePosition(pos);
            }
        }
    }
}