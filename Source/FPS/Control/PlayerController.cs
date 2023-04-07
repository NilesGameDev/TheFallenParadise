using FlaxEngine;
using FPS.Combat;
using FPS.Movement;

namespace FPS.Control
{
    /// <summary>
    /// PlayerController Script.
    /// </summary>
    public class PlayerController : Script
    {

        public bool UseMouse = true;
        public bool CanJump = true;

        public Actor WeaponHolder;

        private PlayerMovement _playerMovement;

        /// <inheritdoc/>
        public override void OnStart()
        {
            base.OnStart();
            if (WeaponHolder == null) 
            {
                Debug.LogWarning("No WeaponHolder is attached to the PlayerController!");
            }
            if (!Actor.TryGetScript(out _playerMovement))
            {
                Debug.LogWarning("No PlayerMovement script is attached to the PlayerController!");
            }
            if (UseMouse)
            {
                Screen.CursorVisible = false;
                Screen.CursorLock = CursorLockMode.Locked;
            }
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            ControlMove();

            if (Input.GetAction("Fire"))
            {
                Attack();
            }
        }

        private void ControlMove()
        {
            float yaw = 0;
            float pitch = 0;

            if (UseMouse)
            {
                yaw = Input.GetAxis("Mouse X");
                pitch = Input.GetAxis("Mouse Y");
            }

            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            var jump = CanJump && Input.GetAction("Jump");

            _playerMovement.AddMovement(horizontal, vertical, pitch, yaw, jump);
        }

        private void Attack()
        {
            if (WeaponHolder.TryGetScript(out WeaponManager script))
            {
                script.CreateBullet();
            }
        }
    }
}
