using PlayerSystems;
using TMPro;
using UnityEngine;

public class IngameUiManager : MonoBehaviour {
    public static IngameUiManager instance { get; private set; }
    [ SerializeField ]
    private TextMeshProUGUI selectedWeaponNameText;
    [ SerializeField ]
    private TextMeshProUGUI selectedWeaponModeText;
    private EquippableWeapon currentWeapon;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        // todo can also check for aiming enabled like this
        if (currentWeapon) {
            WeaponUI();
        }
    }

    private void WeaponUI() {
        if (currentWeapon.HasAlternateMode()) {
            selectedWeaponModeText.text = currentWeapon.GetModeText();
        }
    } 

    public void SelectWeapon(EquippableWeapon weapon) {
        currentWeapon = weapon;
        selectedWeaponNameText.text = currentWeapon.GetName();
    }
}