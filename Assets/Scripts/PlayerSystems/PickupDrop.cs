using Controllers;
using Generics;
using Input;
using UnityEngine;

namespace PlayerSystems {
    public class PickupDrop : MonoBehaviour {
        private HumanoidLandInput input;
        private Transform holdPoint;
        private Camera playerCamera;
        private RaycastHit hit;
        private float range = 3f;
        private Grabbable grabbable;

        void Start() {
            holdPoint = PlayerManager.instance.GetGrabbableHoldPoint();
            playerCamera = CameraController.instance.mainCamera;
            input = HumanoidLandInput.instance;
        }

        void Update() {
            if (input.interactInput) {
                if (grabbable == null) {
                    var ray = playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                    Physics.Raycast(ray, out hit, range);
                    if (hit.transform.TryGetComponent(out grabbable)) {
                        grabbable.Grab(holdPoint);
                    }
                } else {
                    grabbable.Drop();
                    grabbable = null;
                }
            }
        }
    }
}