using UnityEngine;

namespace PlayerSystems {
    public abstract class EquippableWeapon : MonoBehaviour {
        public abstract void Fire();
        public abstract void AltFire();
        public abstract string GetName();
        public abstract bool HasAlternateMode();
        public abstract string GetModeText();
    }
}