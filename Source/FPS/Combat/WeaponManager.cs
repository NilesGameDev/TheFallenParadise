using FlaxEngine;
using FPS.Data.Guns;

namespace FPS.Combat
{
    /// <summary>
    /// WeaponManager Script.
    /// </summary>
    public class WeaponManager : Script
    {
        public GunType Gun;

        [AssetReference(typeof(Gun))]
        public JsonAsset GunAsset;

        private Gun _currentGun;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _currentGun = GunAsset.CreateInstance<Gun>();
            EquipWeapon();
        }
        public override void OnUpdate()
        {
            _currentGun.Tick(Input.Mouse.GetButton(MouseButton.Left));
        }

        public void Fire(bool isInputPressing)
        {
            _currentGun.Tick(isInputPressing);
        }

        public void SelectGun()
        {

        }

        // TODO: Implement this later
        public void EquipWeapon()
        {
            _currentGun.Spawn(Actor, this);
        }
    }
}
