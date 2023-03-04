using TMPro;
using UnityEngine;

public class OnScreenCounter : MonoBehaviour {
    [ SerializeField ]
    private TextMeshProUGUI itemCounter;

    public int counter { get; set; } = 0;

    private void Update() {
        itemCounter.SetText($"Boxes collected: {counter}");
    }
}