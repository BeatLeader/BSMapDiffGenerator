using Parser.Map.Difficulty.V3.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMapDiffGenerator.Models
{
    public readonly record struct DiffEntry(DiffType Type, BeatmapObject Object, CollectionType CollectionType);

    public enum DiffType
    {
        Added,
        Removed,
        Modified
    }

    public enum CollectionType
    {
        BpmEvents,
        Rotations,
        Notes,
        Bombs,
        Walls,
        Arcs,
        Chains,
        Lights,
        ColorBoostBeatmapEvents,
        LightColorEventBoxGroups,
        LightRotationEventBoxGroups,
        LightTranslationEventBoxGroups,
        Other
    }
}
