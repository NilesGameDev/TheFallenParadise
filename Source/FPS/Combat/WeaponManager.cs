using FlaxEngine;

namespace FPS.Combat
{
    /// <summary>
    /// WeaponManager Script.
    /// </summary>
    public class WeaponManager : Script
    {
        public Prefab WeaponPrefab;

        private Actor _currentWeapon;

        /// <inheritdoc/>
        public override void OnStart()
        {
            base.OnStart();
            if (WeaponPrefab == null )
            {
                Debug.LogWarning("Weapon Manager should have a weapon prefab attached!");
                return;
            }
            EquipWeapon();
        }

        public void CreateBullet()
        {
            if (_currentWeapon.TryGetScript(out Weapon script))
            {
                script.SpawnBullet();
            }
        }

        // TODO: Implement this later
        public void EquipWeapon()
        {
            _currentWeapon = PrefabManager.SpawnPrefab(WeaponPrefab, Actor);
        }
    }
}
