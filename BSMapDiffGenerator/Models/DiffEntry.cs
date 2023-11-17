using Parser.Map.Difficulty.V3.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSMapDiffGenerator.Models
{
    public class DiffEntry
    {
        public DiffEntry(DiffType type, BeatmapObject @object, CollectionType collectionType)
        {
            Type = type;
            Object = @object;
            CollectionType = collectionType;
        }

        public DiffType Type { get; set; }
        public BeatmapObject Object { get; set; }
        public CollectionType CollectionType { get; set; }
    }

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
