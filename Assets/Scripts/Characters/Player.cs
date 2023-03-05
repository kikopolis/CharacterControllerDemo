using PlayerSystems;
using UnityEngine;

namespace Characters {
    public class Player : MonoBehaviour, IHealthSystem {
        public void TakeDamage(float damage) {
            Debug.Log("Damaged Player " + damage);
        }
    }
}