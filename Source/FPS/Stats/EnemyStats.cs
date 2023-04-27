using FlaxEngine;

namespace FPS
{
    /// <summary>
    /// EnemyStats Script.
    /// </summary>
    public class EnemyStats : Script
    {
        public float Health = 200f;

        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (Health <= 0 ) 
            {
                Destroy(Actor, 2f);
            }
        }
    }
}
