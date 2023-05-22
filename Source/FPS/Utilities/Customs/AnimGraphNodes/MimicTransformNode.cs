using System;
using FlaxEngine;

/// <summary>
/// MimicTransformNode Script.
/// </summary>
public class MimicTransformNode : AnimationGraph.CustomNode
{
    private Actor _defaultActor;
    private int _nodeIndex;

    public override void Load(ref InitData initData)
    {
        // Here you can access the node value and graph skinned model to setup data
        // This method is called once per node initialization on graph load
        // (usually from the content loading thread)

        // Cache default scale value
        _defaultActor = (Actor)initData.Values[2];
        _nodeIndex = (int)initData.Values[3];
    }

    public override unsafe object Evaluate(ref Context context)
    {
        // Here node is getting called to evaluate the output for the given context

        // Evaluate the input bones pose
        var input = (Impulse*)(IntPtr)GetInputValue(ref context, 0);

        // Evaluate the input Actor
        var actor = HasConnection(ref context, 1) ? (Actor)GetInputValue(ref context, 1) : _defaultActor;
        Debug.Log(actor.Transform);

        // Get the output bones pose (cached internally to improve performance)
        var output = GetOutputImpulseData(ref context);

        // Copy the input and apply the scale to the root node (always the first one)
        CopyImpulseData(input, output);
        output->Nodes[_nodeIndex].Translation = actor.Position;
        output->Nodes[_nodeIndex].Orientation = actor.Orientation;

        // Return the bone pose for further processing
        return new IntPtr(output);
    }
}

