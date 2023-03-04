using UnityEngine;

namespace PlayerSystems {
    public class GunSystem : MonoBehaviour {
        [ SerializeField ]
        private Camera playerCamera;
        [ SerializeField ]
        private Transform attackPoint;
        [ SerializeField ]
        private RaycastHit rayHit;
        [ SerializeField ]
        private LayerMask whatIsEnemy;
        [ SerializeField ]
        private float damage;
        [ SerializeField ]
        private float timeBetweenShooting;
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
        private float bulletsLeft;
        private float bulletsShot;
        private bool shooting;
        private bool readyToShoot;
        private bool reloading;
    }
}