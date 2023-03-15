using Controllers;
using Input;
using UnityEngine;

namespace PlayerSystems {
    public class PickupDrop : MonoBehaviour {
        private HumanoidLandInput input;
        private Transform attackSource;
        private Camera playerCamera;
        private RaycastHit hit;
        private float range = 15f;

        void Start() {
            attackSource = PlayerManager.instance.gunAttackSource;
            playerCamera = CameraController.instance.mainCamera;
            input = HumanoidLandInput.instance;
        }

        void Update() {
            if (input.interactInput != 0) {
                var ray = playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                Physics.Raycast(ray, out hit, range);
                if (hit.collider.GetComponent<EquippableWeapon>()) {
                    
                }
            }
        }
    }
}