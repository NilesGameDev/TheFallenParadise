using FlaxEngine;

namespace FPS.Utilities
{
    /// <summary>
    /// ExitOnEsc Script. Editor only
    /// </summary>
    public class ExitOnEsc : Script
    {
        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (Input.GetKeyUp(KeyboardKeys.Escape)) 
            {
                Engine.RequestExit();
            }
        }
    }
}
