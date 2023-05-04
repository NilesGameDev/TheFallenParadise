using FlaxEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FPS.Core
{
    /// <summary>
    /// WeatherSystem Script.
    /// </summary>
    public class WeatherSystem : Script
    {
        public float BaseColdRate = 0.4f;
        public int ApplyIntervalMs = 400;
        
        public static event Action<float> OnAffectWorld;

        private CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private Task _asyncEmbraceColdnessTask;

        /// <inheritdoc/>
        public override void OnStart()
        {
            _asyncEmbraceColdnessTask = Task.Run(EmbraceExtremeColdness, _cancelToken.Token);
        }
        
        /// <inheritdoc/>
        public override void OnEnable()
        {
            
        }

        /// <inheritdoc/>
        public override void OnDisable()
        {
            // Here you can add code that needs to be called when script is disabled (eg. unregister from events)
        }

        public override void OnDestroy()
        {
            if (_asyncEmbraceColdnessTask != null) {
                _cancelToken.Cancel();
                _asyncEmbraceColdnessTask.Wait(100);
            }
        }

        private async Task EmbraceExtremeColdness()
        {
            while (true)
            {
                try
                {
                    OnAffectWorld?.Invoke(BaseColdRate);
                    await Task.Delay(ApplyIntervalMs, _cancelToken.Token);
                } catch (Exception ex)
                {
                    Debug.LogException(ex);
                    return;
                }

                if (_cancelToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }
    }
}
