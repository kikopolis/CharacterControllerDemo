using PlayerSystems;
using UnityEngine;

namespace Characters {
    public class Enemy : MonoBehaviour, IHealthSystem {
        public void TakeDamage(float damage) {
            Debug.Log("Damaged Enemy" + damage);
        }
    }
}