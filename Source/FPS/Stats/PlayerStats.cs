using FlaxEngine;
using FlaxEngine.GUI;
using FPS.Core;

namespace FPS
{
    /// <summary>
    /// PlayerStats Script.
    /// </summary>
    public class PlayerStats : Script
    {
        [ExpandGroups]
        [EditorOrder(0), EditorDisplay("Stats")]
        public float Health = 100f;
        [EditorOrder(1), EditorDisplay("Stats")]
        public float Warmth = 200f;

        [ExpandGroups]
        [EditorOrder(10), EditorDisplay("UI Components")]
        public UIControl WarmthBar;

        private ProgressBar _warmthBar;
        private float _warmth;

        /// <inheritdoc/>
        public override void OnStart()
        {
            if (WarmthBar == null || !WarmthBar.Is<ProgressBar>())
            {
                Debug.LogWarning("Remember to assign warmth bar UI to the Player!");
            }
            _warmth = Warmth;
            _warmthBar = WarmthBar.Get<ProgressBar>();
            _warmthBar.Value = _warmth;
            _warmthBar.Maximum = _warmth;

            WeatherSystem.OnAffectWorld += ReduceWarmth;
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            UpdateUI();
        }

        private void ReduceWarmth(float baseColdRate)
        {
            _warmth -= baseColdRate;
            _warmth = Mathf.Clamp(_warmth, 0, Warmth);
        }

        private void UpdateUI()
        {
            _warmthBar.Value = _warmth;
        }
    }
}
