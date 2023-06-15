using FlaxEngine;
using FPS.Data.Guns;

namespace FPS.Combat
{
    /// <summary>
    /// WeaponManager Script.
    /// </summary>
    public class WeaponManager : Script
    {
        public Actor WeaponSocket;
        public GunType Gun;

        [AssetReference(typeof(Gun))]
        public JsonAsset GunAsset;

        private Gun _currentGun;

        /// <inheritdoc/>
        public override void OnStart()
        {
            base.OnStart();
            if (WeaponSocket == null)
            {
                Debug.LogWarning("Weapon Manager should have a weapon socket");
            }
            _currentGun = GunAsset.CreateInstance<Gun>();
            EquipWeapon();
        }
        public override void OnUpdate()
        {
            _currentGun.Tick(Input.Mouse.GetButton(MouseButton.Left));
        }

        public void SelectGun()
        {

        }

        public void CreateBullet()
        { 
            //if (_currentGun != null)
            //{
            //    _currentGun.Tick(Input.Mouse.GetButton(MouseButton.Left));
            //}
        }

        // TODO: Implement this later
        public void EquipWeapon()
        {
            _currentGun.Spawn(WeaponSocket, this);
        }
    }
}
