using FlaxEngine;

namespace FPS.Core
{
    /// <summary>
    /// PlayerControlRig Script.
    /// </summary>
    public class PlayerControlRig : Script
    {
        public Actor RightGrip;
        public Actor LeftGrip;

        private AnimatedModel _animatedArms;
        private AnimGraphParameter _rightGripParam;
        private AnimGraphParameter _leftGripParam;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _animatedArms = Actor.As<AnimatedModel>();
            _rightGripParam = _animatedArms.GetParameter("RightGrip");
            _leftGripParam = _animatedArms.GetParameter("LeftGrip");
            _rightGripParam.Value = Transform.WorldToLocal(RightGrip.Transform).Translation;
            _leftGripParam.Value = Transform.WorldToLocal(LeftGrip.Transform).Translation;
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            _rightGripParam.Value = Transform.WorldToLocal(RightGrip.Transform).Translation;
            _leftGripParam.Value = Transform.WorldToLocal(LeftGrip.Transform).Translation;
        }
    }
}
