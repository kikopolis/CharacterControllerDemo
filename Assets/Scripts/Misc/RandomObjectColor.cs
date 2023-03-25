using UnityEngine;

namespace Misc {
    public class RandomObjectColor : MonoBehaviour {
        private void Awake() {
            GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV(
             0f,
             0.1f,
             0.75f,
             1f,
             0.5f,
             1f);
        }
    }
}