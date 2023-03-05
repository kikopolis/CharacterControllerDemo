using UnityEngine;
using Weapons;

namespace PlayerSystems {
    public class WeaponSystem : MonoBehaviour {
        public GravityGun gravityGun { get; private set; }

        private void Awake() {
            gravityGun = gameObject.AddComponent<GravityGun>();
        }
    }
}