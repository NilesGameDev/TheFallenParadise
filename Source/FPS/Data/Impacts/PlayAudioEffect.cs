using FlaxEngine;
using System.Collections.Generic;

namespace FPS.Data.Impacts
{
    [ContentContextMenu("New/Data/Impact System/Play Audio Effect")]
    public class PlayAudioEffect
    {
        public Prefab AudioSourcePrefab;
        public List<AudioClip> AudioClips = new List<AudioClip>();
        [Range(0f, 1f)]
        public Vector2 VolumRange = new Vector2(0, 1);
    }
}
