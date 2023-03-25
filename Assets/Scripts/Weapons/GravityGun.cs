using Controllers;
using Generics;
using Input;
using PlayerSystems;
using UnityEngine;

namespace Weapons{
public class GravityGun : MonoBehaviour, IEquipableOnHotbar{
    private const float GRAVITATIONAL_CONSTANT = 0.667408f;
    private HumanoidLandInput input;
    private float cooldown = 0.2f;
    public float cooldownTimer;
    private float minPushForce = 10f;
    private float maxPushForce = 100f;
    private float forceChargeProgress;
    private float releaseTimerOnRepel = 0.5f;
    // This is the upgradable coefficient for more powerful gravgun
    private float powerLevel = 6f;
    private float holdingRange = 3f;
    private float lerpSpeed = 5f;
    private Transform holdAnchor;
    private Camera playerCamera;
    private RaycastHit hit;
    private string modeText;
    private float maxRange = 30f;
    private ForceMode forceMode = ForceMode.Impulse;
    public float distanceToRb;
    private string grabbableName;
    public Grabbable grabbable;
    public Grabbable temporaryGrabbable;
    public Rigidbody rb;
    public Rigidbody temporaryRb;
    public Mode operationMode = Mode.NEUTRAL;
    public enum Mode{
        ATTRACT, REPEL, NEUTRAL,
    }

    private class Info : IItemInfo{
        public string GetName() {
            return "Gravity Gun";
        }
    }

    private void Start() {
        forceChargeProgress = minPushForce;
        input = HumanoidLandInput.instance;
        playerCamera = CameraController.instance.mainCamera;
        holdAnchor = PlayerManager.instance.GetGrabbableAnchor();
        SetModeText();
    }

    private void FixedUpdate() {
        // General cooldown timer
        if (cooldownTimer > 0) {
            cooldownTimer -= Time.deltaTime;
        }
        // If a grabbable and rb are already existing, check if they are still in range
        if (grabbable && rb) {
            distanceToRb = Vector3.Distance(holdAnchor.position, rb.position);
        } else {
            distanceToRb = 0f;
        }
        // Find a grabbable and set it as a temporary grabbable
        FindGrabbable();
        // If when the player presses the fire button, a grabbable is set and now it needs to be attracted
        if (grabbable && rb && operationMode == Mode.ATTRACT && CooldownExpired()) {
            Attract();
        }
        // If when the player presses the fire button, a grabbable is set and now it needs to be repelled
        if (grabbable && rb && operationMode == Mode.REPEL) {
            Repel();
        }
        // If neutral mode is selected or the rb is out of max range, release and reset
        if (operationMode == Mode.NEUTRAL || distanceToRb > maxRange) {
            Release();
        }
        if (grabbable
         && rb
         && operationMode == Mode.ATTRACT
         && distanceToRb  <= holdingRange
         && operationMode != Mode.NEUTRAL) {
            Hold();
        }
        SetModeText();
    }

    public void Fire() {
        // If a grabbable and a rigidbody are already existing, release them
        if (grabbable && rb && CooldownExpired()) {
            operationMode = Mode.NEUTRAL;
        } else if (temporaryGrabbable && temporaryRb) {
            grabbable = temporaryGrabbable;
            rb = temporaryRb;
            rb.useGravity = false;
            if (rb.interpolation != RigidbodyInterpolation.Interpolate) {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            operationMode = Mode.ATTRACT;
        }
    }

    public void AltFire() {
        // todo implement charging
        if (grabbable && rb && CooldownExpired()) {
            operationMode = Mode.REPEL;
        } else if (temporaryGrabbable && temporaryRb) {
            grabbable = temporaryGrabbable;
            rb = temporaryRb;
            operationMode = Mode.REPEL;
        }
        // Set a release callback
        Invoke(nameof(Release), releaseTimerOnRepel);
    }

    public IItemInfo GetInfo() {
        // return anonymous class
        return new Info();
    }

    private void SetModeText() {
        modeText = "Charge: " + GetPercentage(forceChargeProgress, minPushForce, maxPushForce);
        IngameUiManager.instance.selectedWeaponModeText.text = modeText;
        if (grabbable && rb) {
            grabbableName = grabbable.name;
        } else {
            grabbableName = null;
        }
        if (grabbableName != null) {
            IngameUiManager.instance.interactableObjectNameText.text = grabbableName;
        }
    }

    private void FindGrabbable() {
        // If a grabbable and rb are already existing and in range, return
        if (grabbable && rb) {
            return;
        }
        var ray = playerCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
        Physics.Raycast(ray, out hit, maxRange);
#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * maxRange, Color.magenta, 0.1f);
#endif
        if (hit.collider) {
            hit.transform.TryGetComponent(out temporaryGrabbable);
            hit.transform.TryGetComponent(out temporaryRb);
        } else {
            temporaryGrabbable = null;
            temporaryRb = null;
        }
    }

    private void Attract() {
        if (distanceToRb > holdingRange && distanceToRb < maxRange && operationMode == Mode.ATTRACT) {
            StartCooldown();
            rb.AddForce(CalculateForceForAttract(), forceMode);
        }
    }

    private void Repel() {
        if (distanceToRb < maxRange && operationMode == Mode.REPEL) {
            StartCooldown();
            rb.AddForce(CalculateForceForRepel(), forceMode);
        }
    }

    // todo if player is spamming different directions, the object is lost in space
    private Vector3 CalculateForceForAttract() {
        // If player is facing object, set direction from the hold anchor
        // Vector3 direction;
        // if (Vector3.Dot(holdAnchor.forward, rb.position - holdAnchor.position) > 0) {
        //     direction = -holdAnchor.forward;
        // } else {
        //     direction = rb.position - holdAnchor.position;
        // }
        var direction = -(rb.position - holdAnchor.position);
        var distance = direction.magnitude;
        if (distance == 0f) {
            return Vector3.zero;
        }
        var forceMagnitude
            = 4.5f * (GRAVITATIONAL_CONSTANT * ((powerLevel * forceChargeProgress) * rb.mass) / distance);
        return direction.normalized * forceMagnitude;
    }

    private Vector3 CalculateForceForRepel() {
        var direction = holdAnchor.forward;
        var distance = direction.magnitude;
        if (distance == 0f) {
            return Vector3.zero;
        }
        var forceMagnitude
            = 0.10f * (GRAVITATIONAL_CONSTANT * ((powerLevel * forceChargeProgress) * rb.mass) / distance);
        return direction.normalized * forceMagnitude;
    }

    private void Hold() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        var pos = Vector3.Lerp(rb.position, holdAnchor.position, Time.deltaTime * lerpSpeed * powerLevel);
        rb.MovePosition(pos);
    }

    private void Release() {
        if (!rb || !grabbable) {
            return;
        }
        rb.useGravity = true;
        temporaryGrabbable = null;
        temporaryRb = null;
        grabbable = null;
        rb = null;
        operationMode = Mode.NEUTRAL;
    }

    private bool CooldownExpired() {
        return cooldownTimer <= 0;
    }

    private void StartCooldown() {
        cooldownTimer = cooldown;
    }

    private int GetPercentage(float value, float min, float max) {
        return Mathf.RoundToInt((value - min) / (max - min) * 100);
    }
}
}