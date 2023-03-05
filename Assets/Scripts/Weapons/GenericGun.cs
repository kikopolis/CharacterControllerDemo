using Input;
using PlayerSystems;
using UnityEngine;

namespace Weapons {
    public class GenericGun : MonoBehaviour {
        [ SerializeField ]
        private HumanoidLandInput input;
        [ SerializeField ]
        private Rigidbody playerRigidBody;
        [ SerializeField ]
        private Transform bulletSource;
        [ SerializeField ]
        private LayerMask whatIsEnemy;
        [ SerializeField ]
        private float damage;
        [ SerializeField ]
        private float shotResetTime;
        [ SerializeField ]
        private float spread;
        [ SerializeField ]
        private float range;
        [ SerializeField ]
        private float reloadTime;
        [ SerializeField ]
        private float timeBetweenShots;
        [ SerializeField ]
        private float magazineSize;
        [ SerializeField ]
        private float bulletsPerTap;
        [ SerializeField ]
        private bool allowButtonHold;
        private RaycastHit rayHit;
        private float bulletsLeft;
        private float bulletsShot;
        private bool readyToShoot;
        private bool reloading;

        [ Header("Graphics") ]
        [ SerializeField ]
        private GameObject bulletHoleGraphic;
        [ SerializeField ]
        private GameObject muzzleFlashGraphic;

        private void Awake() {
            bulletsLeft = magazineSize;
            readyToShoot = true;
        }

        private void FixedUpdate() {
            if (input.reloadInput && bulletsLeft < magazineSize && !reloading) {
                Reload();
            }
            if (readyToShoot && input.fireInput && !reloading && bulletsLeft > 0f) {
                bulletsShot = bulletsPerTap;
                Shoot();
            }
        }

        private void Reload() {
            reloading = true;
            Invoke("ReloadFinished", reloadTime);
        }

        private void ReloadFinished() {
            bulletsLeft = magazineSize;
            reloading = false;
        }

        private void Shoot() {
            readyToShoot = false;

            // Set Spread dependent on player movement
            var finalSpread = spread;
            if (playerRigidBody.velocity.magnitude > 0) {
                finalSpread = spread * 2;
            }

            // Direction with the spread
            var x = Random.Range(-finalSpread, finalSpread);
            var y = Random.Range(-finalSpread, finalSpread);
            var direction = bulletSource.forward + new Vector3(x, y, 0f);

            // Raycast to determine the target hit
            if (Physics.Raycast(bulletSource.position, direction, out rayHit, range, whatIsEnemy)) {
                if (rayHit.collider.CompareTag("Damageable")) {
                    rayHit.collider.GetComponent<IHealthSystem>().TakeDamage(damage);
                }
            }

            // Graphics
            Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
            Instantiate(muzzleFlashGraphic, bulletSource.position, Quaternion.identity);

            bulletsLeft--;
            bulletsShot--;
            Invoke("ResetShot", shotResetTime);
            if (bulletsShot > 0f && bulletsLeft > 0f) {
                Invoke("Shoot", timeBetweenShots);
            }
        }

        private void ResetShot() {
            readyToShoot = true;
        }
    }
}