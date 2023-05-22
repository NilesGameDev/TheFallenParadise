using FlaxEditor.Surface;
using FlaxEngine;

namespace FPS
{
    /// <summary>
    /// CustomAnimGraphNodeFactory Script.
    /// </summary>
    [AnimationGraph.CustomNodeArchetypeFactory]
    public static class CustomAnimGraphNodeFactory
    {
        public static NodeArchetype GetActorTransformExtractorNodeArchetype()
        {
            return new NodeArchetype
            {
                // Define node title and metadata
                Title = "Mimic Transform",
                Description = "",
                Flags = NodeFlags.AnimGraph,

                // Define node variables (per instance)
                // DefaultValues[0] must specify the C# runtime controller typename
                // DefaultValues[1] must specify the node group name
                // use other slots to store custom data per-node
                DefaultValues = new object[]
                {
                "MimicTransformNode", // Runtime node typename
                "CustomNodes", // Group name
                new EmptyActor(), // Custom value stored per node
                      // ..here you can store more per-node data
                -1
                },
                
                // Define node visuals and elements
                Size = new Float2(250, 70),
                Elements = new[]
                {
                    NodeElementArchetype.Factory.Input(0, "Input", true, typeof(void), 0),
                    NodeElementArchetype.Factory.Input(1, "Actor", true, typeof(Actor), 1, 2),

                    NodeElementArchetype.Factory.Text(1, Constants.LayoutOffsetY * 2, "Target Node: "),
                    NodeElementArchetype.Factory.SkeletonBoneIndexSelect(Constants.BoxSize * 4, Constants.LayoutOffsetY * 2, 120, 3),

                    NodeElementArchetype.Factory.Output(0, "Output", typeof(void), 2),
                },
            };
        }
    }
}
