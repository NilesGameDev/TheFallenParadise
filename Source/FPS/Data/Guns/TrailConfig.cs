using FlaxEngine;

namespace FPS.Data.Guns
{
    [ContentContextMenu("New/Data/Guns/TrailConfig")]
    public class TrailConfig
    {
        public Material Material;
        public BezierCurve<float> WidthCurve = new BezierCurve<float>();
        public float Duration = 0.5f;
        public float MinVertexDistance = 0.1f;
        public Color Color;
        public float MissDistance = 100f;
        public float SimulationSpeed = 100f;
    }
}
