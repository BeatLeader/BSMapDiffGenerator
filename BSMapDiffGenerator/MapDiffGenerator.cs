using BSMapDiffGenerator.Models;
using Parser.Map;
using Parser.Map.Difficulty.V3.Base;
using Parser.Map.Difficulty.V3.Event;
using Parser.Map.Difficulty.V3.Event.V3;
using Parser.Map.Difficulty.V3.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BSMapDiffGenerator
{
    public class MapDiffGenerator
    {
        public static MapDiff GenerateFullDiff()
        {
            return new(new(), new());
        }

        public static List<DiffEntry> GenerateSongInfoDiff()
        {
            return new();
        }

        public static List<DiffEntry> GenerateDifficultyDiff(DifficultyV3 newDifficulty, DifficultyV3 oldDifficulty)
        {
            List<DiffEntry> diffEntries = new();

            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.bpmEvents.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.bpmEvents.ConvertAll(obj => obj as BeatmapObject), CollectionType.BpmEvents));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Rotations.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Rotations.ConvertAll(obj => obj as BeatmapObject), CollectionType.Rotations));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Notes.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Notes.ConvertAll(obj => obj as BeatmapObject), CollectionType.Notes));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Bombs.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Bombs.ConvertAll(obj => obj as BeatmapObject), CollectionType.Bombs));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Walls.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Walls.ConvertAll(obj => obj as BeatmapObject), CollectionType.Walls));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Arcs.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Arcs.ConvertAll(obj => obj as BeatmapObject), CollectionType.Arcs));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Chains.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Chains.ConvertAll(obj => obj as BeatmapObject), CollectionType.Chains));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.Lights.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.Lights.ConvertAll(obj => obj as BeatmapObject), CollectionType.Lights));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.colorBoostBeatmapEvents.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.colorBoostBeatmapEvents.ConvertAll(obj => obj as BeatmapObject), CollectionType.ColorBoostBeatmapEvents));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.lightColorEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.lightColorEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), CollectionType.LightColorEventBoxGroups));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.lightRotationEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.lightRotationEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), CollectionType.LightRotationEventBoxGroups));
            diffEntries.AddRange(GenerateObjectsDiff(newDifficulty.lightTranslationEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), oldDifficulty.lightTranslationEventBoxGroups.ConvertAll(obj => obj as BeatmapObject), CollectionType.LightTranslationEventBoxGroups));

            return diffEntries;
        }

        private static List<DiffEntry> GenerateObjectsDiff(List<BeatmapObject> newObjects, List<BeatmapObject> oldObjects, CollectionType collectionType)
        {
            List<DiffEntry> diffEntries = new();
            List<(BeatmapObject obj, bool stillExists)> oldObjsChecked = oldObjects.ConvertAll(obj => (obj, false));

            // checking if new notes have a match in old notes
            Parallel.ForEach(newObjects, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, obj =>
            {
                var matches = CheckMatches(obj, oldObjects);

                if (matches.Count() > 0)
                {
                    foreach (var match in matches)
                    {
                        int index = oldObjsChecked.FindIndex(x => !x.stillExists && match.Equals(x.obj));
                        if (index != -1) oldObjsChecked[index] = (match, true);
                    }
                }
                else
                {
                    // adding new notes
                    diffEntries.Add(new DiffEntry(DiffType.Added, obj, collectionType));
                }
            });

            // adding removed notes
            Parallel.ForEach(oldObjsChecked.Where(x => !x.stillExists), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, obj =>
            {
                diffEntries.Add(new DiffEntry(DiffType.Removed, obj.obj, collectionType));
            });

            return diffEntries;
        }

        private static List<T> CheckMatches<T>(T obj, List<T> oldObjects)
        {
            return oldObjects.Where(x => obj.Equals(x)).ToList();
        }
    }
}
