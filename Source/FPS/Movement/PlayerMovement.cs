using System;
using FlaxEngine;

namespace FPS.Movement
{
    /// <summary>
    /// PlayerMovement Script.
    /// </summary>
    public class PlayerMovement : Script
    {
        [EditorOrder(0)]
        public CharacterController PlayerController;

        [EditorOrder(5), EditorDisplay("Camera Configs")]
        public Actor CameraTarget;
        [EditorOrder(6), EditorDisplay("Camera Configs")]
        public Camera FPSCamera;
        [EditorOrder(7), EditorDisplay("Camera Configs", "Camera Pitch Range")]
        public Float2 PitchMinMax = new Float2(-88, 88);
        [EditorOrder(8), EditorDisplay("Camera Configs")]
        public float CameraSmoothing = 20.0f;
        [EditorOrder(9), EditorDisplay("Camera Configs", "Use Camera Smoothing")]
        public bool IsMotionBlur = true;

        [EditorOrder(10), EditorDisplay("Movement Params")]
        public float JumpForce = 800;
        [EditorOrder(11), EditorDisplay("Movement Params")]
        public float Friction = 8.0f;
        [EditorOrder(12), EditorDisplay("Movement Params")]
        public float GroundAccelerate = 5000;
        [EditorOrder(13), EditorDisplay("Movement Params")]
        public float AirAccelerate = 10000;
        [EditorOrder(14), EditorDisplay("Movement Params")]
        public float MaxVelocityGround = 400;
        [EditorOrder(15), EditorDisplay("Movement Params")]
        public float MaxVelocityAir = 200;

        private Vector3 _velocity;
        private bool _jump;
        private float _pitch;
        private float _yaw;
        private float _horizontal;
        private float _vertical;

        /// <summary>
        /// Adds the movement and rotation to the camera (as input).
        /// </summary>
        /// <param name="horizontal">The horizontal input.</param>
        /// <param name="vertical">The vertical input.</param>
        /// <param name="pitch">The pitch rotation input.</param>
        /// <param name="yaw">The yaw rotation input.</param>
        /// <param name="jump">The boolean indicating whether to jump.</param>
        public void AddMovement(float horizontal, float vertical, float pitch, float yaw, bool jump)
        {
            _pitch = Mathf.Clamp(_pitch + pitch, PitchMinMax.MinValue, PitchMinMax.MaxValue);
            _yaw += yaw;
            _horizontal += horizontal;
            _vertical += vertical;
            _jump = jump;
        }

        public override void OnFixedUpdate()
        {
            // Update camera
            var camTrans = FPSCamera.Transform;
            var camFactor = Mathf.Saturate(CameraSmoothing * Time.DeltaTime);
            CameraTarget.LocalOrientation = IsMotionBlur
                ? Quaternion.Lerp(CameraTarget.LocalOrientation, Quaternion.Euler(_pitch, _yaw, 0), camFactor)
                : Quaternion.Euler(_pitch, _yaw, 0);
            camTrans.Translation = Vector3.Lerp(camTrans.Translation, CameraTarget.Position, camFactor);
            camTrans.Orientation = CameraTarget.Orientation;
            FPSCamera.Transform = camTrans;

            var inputH = _horizontal;
            var inputV = _vertical;
            _horizontal = 0;
            _vertical = 0;

            var velocity = new Vector3(inputH, 0.0f, inputV);
            velocity.Normalize();
            Vector3 rotation = CameraTarget.LocalEulerAngles;
            rotation.X = 0;
            rotation.Z = 0;
            velocity = Vector3.Transform(velocity, Quaternion.Euler(rotation));

            if (PlayerController.IsGrounded)
            {
                velocity = MoveGround(velocity.Normalized, Horizontal(_velocity));
                velocity.Y = -Mathf.Abs(Physics.Gravity.Y * 0.5f);
            }
            else
            {
                velocity = MoveAir(velocity.Normalized, Horizontal(_velocity));
                velocity.Y = _velocity.Y;
            }

            // Fix direction
            if (velocity.Length < 0.05f)
                velocity = Vector3.Zero;

            // Jump
            if (_jump && PlayerController.IsGrounded)
                velocity.Y = JumpForce;
            _jump = false;

            // Apply gravity
            velocity.Y += -Mathf.Abs(Physics.Gravity.Y * 2.5f) * Time.DeltaTime;

            // Check if player is not blocked by something above head
            if ((PlayerController.Flags & CharacterController.CollisionFlags.Above) != 0)
            {
                if (velocity.Y > 0)
                {
                    // Player head hit something above, zero the gravity acceleration
                    velocity.Y = 0;
                }
            }

            // Move
            PlayerController.Move(velocity * Time.DeltaTime);
            _velocity = velocity;
        }

        private Vector3 Horizontal(Vector3 v)
        {
            return new Vector3(v.X, 0, v.Z);
        }

        // accelDir: normalized direction that the player has requested to move (taking into account the movement keys and look direction)
        // prevVelocity: The current velocity of the player, before any additional calculations
        // accelerate: The server-defined player acceleration value
        // maxVelocity: The server-defined maximum player velocity (this is not strictly adhered to due to strafejumping)
        private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
        {
            float projVel = (float)Vector3.Dot(prevVelocity, accelDir); // Vector projection of Current velocity onto accelDir
            float accelVel = accelerate * Time.DeltaTime; // Accelerated velocity in direction of movement

            // If necessary, truncate the accelerated velocity so the vector projection does not exceed max velocity
            if (projVel + accelVel > maxVelocity)
                accelVel = maxVelocity - projVel;

            return prevVelocity + accelDir * accelVel;
        }

        private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
        {
            // Apply Friction
            var speed = prevVelocity.Length;
            if (Math.Abs(speed) > 0.01f) // To avoid divide by zero errors
            {
                var drop = speed * Friction * Time.DeltaTime;
                prevVelocity *= Mathf.Max(speed - drop, 0) / speed; // Scale the velocity based on friction
            }

            // GroundAccelerate and MaxVelocityGround are server-defined movement variables
            return Accelerate(accelDir, prevVelocity, GroundAccelerate, MaxVelocityGround);
        }

        private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
        {
            // air_accelerate and max_velocity_air are server-defined movement variables
            return Accelerate(accelDir, prevVelocity, AirAccelerate, MaxVelocityAir);
        }

        public override void OnDebugDraw()
        {
            var trans = PlayerController.Transform;
            DebugDraw.DrawWireTube(trans.Translation, trans.Orientation * Quaternion.Euler(90, 0, 0), PlayerController.Radius, PlayerController.Height, Color.Blue);
        }
    }
}
