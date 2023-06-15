// Copyright © 2022 PrecisionRender. All rights reserved.
using FlaxEngine;

using FPS.Core;

namespace FPS
{
    /// <summary>
    /// FirstPersonCamera Script.
    /// </summary>
    public class FirstPersonCamera : Script
    {
        public float CameraSmoothSpeed = 30;

        [Serialize, ShowInEditor, EditorOrder(5), EditorDisplay(name: "Pitch Limit")]
        private Vector2 pitchLimit = new Vector2(-88, 88);

        private CharacterControllerPro _playerController;

        private float _targetPitch = 0;
        private float _currentPitch = 0;
        private float _currentYaw = 0;
        private float _inputPitch;
        private float _inputYaw;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _playerController = Actor.Parent.GetScript<CharacterControllerPro>();
        }

        public void AddRotationInput(float inputYaw, float inputPitch)
        {
            _inputPitch = inputPitch;
            _inputYaw = inputYaw;
        }

        public override void OnUpdate()
        {
            // Get look input
            Vector2 lookInput = new Vector2(_inputYaw, _inputPitch);

            _targetPitch += lookInput.Y;
            // Clamp target pitch to keep from looking upside down
            _targetPitch = Mathf.Clamp(_targetPitch, pitchLimit.X, pitchLimit.Y);

            // Add character rotation
            _playerController.AddCharacterRotation(new Vector3(0, lookInput.X, 0));

            // Interpolate camera arm towards the desired rotation
            var camFactor = Mathf.Saturate(CameraSmoothSpeed * Time.DeltaTime);
            _currentPitch = Mathf.SmoothStep(_currentPitch, _targetPitch, camFactor);
            _currentYaw = Mathf.SmoothStep(_currentYaw, _playerController.CharacterRotation.Y, camFactor);

            // Apply rotation
            var headTrans = Actor.Transform;
            Actor.LocalOrientation = Quaternion.Lerp(Actor.LocalOrientation, Quaternion.Euler(_currentPitch, _currentYaw, 0), camFactor);
            headTrans.Orientation = Actor.LocalOrientation;
            Actor.Transform = headTrans;
        }
    }
}
