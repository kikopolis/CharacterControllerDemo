using UnityEngine;

public class GameManager : MonoBehaviour {
    private bool vsyncEnabled;
    private int targetFramerate = 60;
    private float targetPhysicsRate = 60f;

    private void Awake() {
        QualitySettings.vSyncCount = vsyncEnabled ? 1 : 0;
        Application.targetFrameRate = targetFramerate;
        Time.fixedDeltaTime = 1f / targetPhysicsRate;
        Time.maximumDeltaTime = Time.fixedDeltaTime * 1.25f;
    }

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}