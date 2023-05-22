using FlaxEngine;

namespace FPS
{
    /// <summary>
    /// PlayerControlRig Script.
    /// </summary>
    public class PlayerControlRig : Script
    {
        public Actor TargetWeapon;
        public Actor TargetWeapon2;

        private AnimatedModel _animatedArms;
        private AnimGraphParameter _targetWeaponParam;
        private AnimGraphParameter _targetWeapon2Param;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _animatedArms = Actor.As<AnimatedModel>();
            _targetWeaponParam = _animatedArms.GetParameter("TargetWeapon");
            _targetWeapon2Param = _animatedArms.GetParameter("TargetWeapon2");
            _targetWeaponParam.Value = Transform.WorldToLocal(TargetWeapon.Transform).Translation;
            _targetWeapon2Param.Value = Transform.WorldToLocal(TargetWeapon2.Transform).Translation;
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            // Here you can add code that needs to be called when script is enabled (eg. register for events)
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            _targetWeaponParam.Value = Transform.WorldToLocal(TargetWeapon.Transform).Translation;
            _targetWeapon2Param.Value = Transform.WorldToLocal(TargetWeapon2.Transform).Translation;
        }
    }
}
