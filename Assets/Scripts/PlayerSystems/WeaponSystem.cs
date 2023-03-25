using UnityEngine;
using Weapons;

namespace PlayerSystems{
public class WeaponSystem : MonoBehaviour{
    [SerializeField]
    private GravityGun gravityGun;
    
    public GravityGun GetGravityGun(){
        return gravityGun;
    }
}
}