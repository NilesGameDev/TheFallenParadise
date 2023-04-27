using FlaxEngine;
using FPS.Core;
using FPS.Combat;
using FPS.Movement;

namespace FPS.Control
{
    /// <summary>
    /// PlayerInput Script.
    /// </summary>
    public class PlayerInput : Script
    {
        [Serialize, ShowInEditor, EditorDisplay(name: "Horizontal Input")]
        private bool horizontalInput = false;

        public bool UseMouse = true;
        public bool CanJump = true;
        public Actor WeaponHolder;

        private CharacterControllerPro _playerController;
        private FirstPersonCamera _fpsCamera;
        private FpsMotion _fpsMotion;

        /// <inheritdoc/>
        public override void OnStart()
        {
            var className = GetType().Name;
            if (!Actor.TryGetScript(out _playerController))
            {
                Debug.LogWarning($"No CharacterControllerPro script is attached to the {className}!");
            }
            if (WeaponHolder == null) 
            {
                Debug.LogWarning($"No WeaponHolder is attached to the {className}!");
            }
            if (Actor.GetChild("Head") == null || !Actor.GetChild("Head").TryGetScript(out _fpsCamera))
            {
                Debug.LogWarning($"This Actor should have a child actor named 'Head' with a script FirstPersonCamera attached!");
            }
            if (!Actor.TryGetScript(out _fpsMotion))
            {
                Debug.LogWarning($"No FpsMotion script is attached to the {className}!");
            }
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (UseMouse)
            {
                Screen.CursorVisible = false;
                Screen.CursorLock = CursorLockMode.Locked;
            }

            InteractMotion();
            InteractCombat();
        }

        private void InteractMotion()
        {
            GetRotationInput(out float yaw, out float pitch);
            _fpsCamera.AddRotationInput(yaw, pitch);

            // Get forward and right direction based on the charcater's CharcaterRotation
            Vector3 forwardDirection = Vector3.Transform(Vector3.Forward, Quaternion.Euler(_playerController.CharacterRotation));
            Vector3 rightDirection = Vector3.Transform(Vector3.Right, Quaternion.Euler(_playerController.CharacterRotation));
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // Add movement in those directions
            if (!horizontalInput)
            {
                _playerController.AddMovementInput(forwardDirection, vertical);
            }
            _playerController.AddMovementInput(rightDirection, horizontal);

            // Trigger jumping
            if (Input.GetAction("Jump"))
            {
                _playerController.Jump();
            }
            if (Input.GetAction("Stop Jump"))
            {
                _playerController.StopJumping();
            }

            // Trigger running
            if (Input.GetAction("Sprint"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Running;
            }
            if (Input.GetAction("Stop Sprint"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Walking;
            }

            // Trigger crouching
            if (Input.GetAction("Crouch"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Crouching;
            }
            if (Input.GetAction("Stop Crouch"))
            {
                _playerController.MovementMode = CharacterControllerPro.MovementModes.Uncrouching;
            }

            _fpsMotion.AddMotionInput(new Vector3(yaw, pitch, 0), new Vector3(horizontal, vertical, 0));
        }

        private void InteractCombat()
        {
            if (Input.GetAction("Fire"))
            {
                Attack();
            }
        }

        private void GetRotationInput(out float yaw, out float pitch)
        {
            yaw = 0;
            pitch = 0;
            if (UseMouse)
            {
                yaw = Input.GetAxis("Mouse X");
                pitch = Input.GetAxis("Mouse Y");
            }
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
