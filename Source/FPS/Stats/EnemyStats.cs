using FlaxEngine;
using FlaxEngine.GUI;

namespace FPS
{
    /// <summary>
    /// EnemyStats Script.
    /// </summary>
    public class EnemyStats : Script
    {
        [Limit(0)]
        public float Health = 200f;
        public UIControl HealthBar;

        private ProgressBar _healthBar;

        public override void OnStart()
        {
            if (HealthBar == null || !HealthBar.Is<ProgressBar>())
            {
                Debug.LogWarning("Remember to assign warmth bar UI to the Player!");
            }
            _healthBar = HealthBar.Get<ProgressBar>();
            _healthBar.Maximum = Health;
            _healthBar.Value = Health;
        }

        public override void OnUpdate()
        {
            UpdateUI();
        }


        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0 ) 
            {
                Destroy(Actor, 2f);
            }
        }

        private void UpdateUI()
        {
            _healthBar.Value = Health;
        }
    }
}
