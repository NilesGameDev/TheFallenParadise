using FlaxEngine;
using FPS.Core;
using FPS.Data.Guns;

namespace FPS.Combat
{
    /// <summary>
    /// WeaponManager Script.
    /// </summary>
    public class WeaponManager : Script
    {
        public GunType Gun;
        public Actor MainArms;

        [AssetReference(typeof(Gun))]
        public JsonAsset GunAsset;

        private Gun _currentGun;
        private PlayerControlRig _controlRig;

        /// <inheritdoc/>
        public override void OnStart()
        {
            if (MainArms is null)
            {
                Debug.LogWarning("MainArms not assigned in WeaponManager!");
                return;
            }

            _currentGun = GunAsset.CreateInstance<Gun>();
            _controlRig = MainArms.GetScript<PlayerControlRig>();
            EquipWeapon();
        }
        public override void OnUpdate()
        {
            if (_currentGun is null) return;
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
            _currentGun.Spawn(Actor, this, out _controlRig.RightGrip, out _controlRig.LeftGrip);
        }
    }
}
