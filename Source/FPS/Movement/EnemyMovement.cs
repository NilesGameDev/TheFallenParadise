using System;
using FlaxEngine;

namespace FPS.Movement
{
    /// <summary>
    /// EnemyMovement Script.
    /// </summary>
    public class EnemyMovement : Script
    {

        [Tooltip("The speed of the object animation over the spline.")]
        public float Speed = 1.0f;

        [Tooltip("The actor to move it over the spline.")]
        public Actor TargetActor;

        private float _time;
        private Spline _spline;

        public override void OnEnable()
        {
            // Cache spline actor
            _spline = Actor.As<Spline>();
            if (!_spline)
                throw new Exception("Attach script to a spline.");
        }

        public override void OnUpdate()
        {
            if (!_spline || !TargetActor)
                return;

            // Update position
            _time += Time.DeltaTime * Speed;

            // Evaluate the spline curve
            var direction = _spline.GetSplineDirection(_time);
            var transform = _spline.GetSplineTransform(_time);

            // Place object on the spline and make it oriented along the spline direction
            transform.Orientation = Quaternion.LookRotation(direction, Float3.Up) * transform.Orientation;
            TargetActor.Transform = transform;
        }
    }
}
