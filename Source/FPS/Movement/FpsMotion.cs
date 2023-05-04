using FlaxEngine;

namespace FPS.Movement
{
    /// <summary>
    /// FpsMotion Script. Used to perform swaying on camera move and bobbing on player move
    /// </summary>
    public class FpsMotion : Script
    {
        [ExpandGroups]
        [EditorOrder(0), EditorDisplay("General Settings")]
        public CharacterController Player;
        [EditorOrder(1), EditorDisplay("General Settings")]
        public Actor WeaponArm;
        [EditorOrder(2), EditorDisplay("General Settings")]
        public bool AllowSway;
        [EditorOrder(3), EditorDisplay("General Settings")]
        public bool AllowBob;
        [EditorOrder(4), EditorDisplay("General Settings")]
        public float Smooth = 10f;
        [EditorOrder(5), EditorDisplay("General Settings")]
        public float SmoothRotation = 12f;

        [ExpandGroups]
        [EditorOrder(10), EditorDisplay("Sway Settings")]
        public float Step = 0.01f;
        [EditorOrder(11), EditorDisplay("Sway Settings")]
        public Float2 StepDistanceMinMax = new Float2(-0.06f, 0.06f);
        [EditorOrder(12), EditorDisplay("Sway Settings")]
        public float RotationStep = 4f;
        [EditorOrder(13), EditorDisplay("Sway Settings")]
        public Float2 RotationStepDistanceMinMax = new Float2(-5f, 5f);

        [ExpandGroups]
        [EditorOrder(20), EditorDisplay("Bob Settings")]
        public float SpeedCurve;
        [EditorOrder(21), EditorDisplay("Bob Settings")]
        public Vector3 TravelLimit = Vector3.One * 0.025f;
        [EditorOrder(22), EditorDisplay("Bob Settings")]
        public Vector3 BobLimit = Vector3.One * 0.01f;
        [EditorOrder(23), EditorDisplay("Bob Settings")]
        public Vector3 Multiplier = Vector3.One * 2;
        [EditorOrder(24), EditorDisplay("Bob Settings"), ExpandGroups]
        public float bobExaggeration;
        
        private float _curveSin { get => Mathf.Sin(SpeedCurve); }
        private float _curveCos { get => Mathf.Cos(SpeedCurve); }

        private Vector3 _swayPos;
        private Vector3 _swayEuler;
        private Vector3 _bobPos;
        private Vector3 _bobEuler;

        private Vector3 _lookInput;
        private Vector3 _walkInput;

        public override void OnStart()
        {
            _swayPos = Vector3.Zero;
            _bobPos = Vector3.Zero;

            if (Player is null)
            {
                Debug.LogWarning("Should attach a Player actor to FpsMotion script!");
            }

            if (WeaponArm is null)
            {
                Debug.LogWarning("Should attach a Weapon Holder actor to FpsMotion script!");
            }
        }

        public override void OnUpdate()
        {
            if (AllowSway)
            {
                ApplySway();
                ApplySwayRotation();
            }

            SpeedCurve += Time.DeltaTime * (Player.IsGrounded ? Player.Velocity.Length * bobExaggeration : 1f);
            
            if (AllowBob)
            {
                ApplyBob();
                ApplyBobRotation();
            }
            Composite();
        }

        public void AddMotionInput(Vector3 lookInput, Vector3 walkInput)
        {
            _lookInput = lookInput;
            _walkInput = walkInput.Normalized;
        }

        private void Composite()
        {
            var armTrans = WeaponArm.Transform;
            armTrans.WorldToLocal(Vector3.Lerp(armTrans.Translation, _swayPos + _bobPos, Time.DeltaTime * Smooth));
            WeaponArm.LocalOrientation = Quaternion.Slerp(
                WeaponArm.LocalOrientation, 
                Quaternion.Euler(_swayEuler) * Quaternion.Euler(_bobEuler), 
                Time.DeltaTime * SmoothRotation);
            armTrans.Orientation = WeaponArm.Orientation;
            WeaponArm.Transform = armTrans;
        }

        private void ApplySway()
        {
            Vector3 invertLook = _lookInput * -Step;
            invertLook.X = Mathf.Clamp(invertLook.X, StepDistanceMinMax.MinValue, StepDistanceMinMax.MaxValue);
            invertLook.Y = Mathf.Clamp(invertLook.Y, StepDistanceMinMax.MinValue, StepDistanceMinMax.MaxValue);

            _swayPos = invertLook;
        }

        private void ApplySwayRotation()
        {
            Vector3 invertLook = _lookInput * -RotationStep;
            invertLook.X = Mathf.Clamp(invertLook.X, RotationStepDistanceMinMax.MinValue, RotationStepDistanceMinMax.MaxValue);
            invertLook.Y = Mathf.Clamp(invertLook.Y, RotationStepDistanceMinMax.MinValue, RotationStepDistanceMinMax.MaxValue);

            _swayEuler = new Vector3(invertLook.Y, invertLook.X, invertLook.X);
        }

        private void ApplyBob()
        {
            _bobPos.X = (_curveCos * BobLimit.X * (Player.IsGrounded ? 1 : 0)) - (_walkInput.X * TravelLimit.X);
            _bobPos.Y = (_curveSin * BobLimit.Y) - (Player.Velocity.Y * TravelLimit.Y);
            _bobPos.Z = -(_walkInput.Y * TravelLimit.Z);
        }

        private void ApplyBobRotation()
        {
            var isMoving = _walkInput != Vector3.Zero;

            _bobEuler.X = isMoving ? (Multiplier.X * Mathf.Sin(2 * SpeedCurve)) : (Multiplier.X * (Mathf.Sin(2 * SpeedCurve) / 2));
            _bobEuler.Y = isMoving ? (Multiplier.Y * _curveCos) : 0;
            _bobEuler.Z = isMoving ? Multiplier.Z * _curveCos * _walkInput.X : 0;
        }
    }
}
