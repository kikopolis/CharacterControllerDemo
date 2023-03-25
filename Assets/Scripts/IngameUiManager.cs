using PlayerSystems;
using TMPro;
using UnityEngine;

public class IngameUiManager : MonoBehaviour {
    public static IngameUiManager instance { get; private set; }
    [ SerializeField ]
    public TextMeshProUGUI selectedWeaponNameText;
    [ SerializeField ]
    public TextMeshProUGUI selectedWeaponModeText;
    [ SerializeField ]
    public TextMeshProUGUI interactableObjectNameText;
    private EquippableWeapon currentWeapon;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // todo can also check for aiming enabled like this
        if (currentWeapon) {
            WeaponUI();
        }
        // todo implement interactable object ui
    }

    private void WeaponUI() {
        selectedWeaponModeText.enabled = true;
    } 

    public void SelectWeapon(EquippableWeapon weapon) {
        currentWeapon = weapon;
        selectedWeaponNameText.text = currentWeapon.GetName();
    }
}