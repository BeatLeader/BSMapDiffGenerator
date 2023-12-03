using BSMapDiffGenerator.Models;
using Parser.Map.Difficulty.V3.Base;
using Parser.Map.Difficulty.V3.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BSMapDiffGenerator
{
    public class MapDiffGenerator
    {
        public static MapDiff GenerateFullDiff() => throw new NotImplementedException();

        public static List<DiffEntry> GenerateSongInfoDiff() => throw new NotImplementedException();

        public static List<DiffEntry> GenerateDifficultyDiff(DifficultyV3 newDifficulty, DifficultyV3 oldDifficulty) => [
            .. GenerateObjectsDiff(newDifficulty.bpmEvents, oldDifficulty.bpmEvents, CollectionType.BpmEvents),
            .. GenerateObjectsDiff(newDifficulty.Rotations, oldDifficulty.Rotations, CollectionType.Rotations),
            .. GenerateObjectsDiff(newDifficulty.Notes.Where(x => x.Color == 0).ToList(), oldDifficulty.Notes.Where(x => x.Color == 0).ToList(), CollectionType.Notes),
            .. GenerateObjectsDiff(newDifficulty.Notes.Where(x => x.Color == 1).ToList(), oldDifficulty.Notes.Where(x => x.Color == 1).ToList(), CollectionType.Notes),
            .. GenerateObjectsDiff(newDifficulty.Bombs, oldDifficulty.Bombs, CollectionType.Bombs),
            .. GenerateObjectsDiff(newDifficulty.Walls, oldDifficulty.Walls, CollectionType.Walls),
            .. GenerateObjectsDiff(newDifficulty.Arcs.Where(x => x.Color == 0).ToList(), oldDifficulty.Arcs.Where(x => x.Color == 0).ToList(), CollectionType.Arcs),
            .. GenerateObjectsDiff(newDifficulty.Arcs.Where(x => x.Color == 1).ToList(), oldDifficulty.Arcs.Where(x => x.Color == 1).ToList(), CollectionType.Arcs),
            .. GenerateObjectsDiff(newDifficulty.Chains.Where(x => x.Color == 0).ToList(), oldDifficulty.Chains.Where(x => x.Color == 0).ToList(), CollectionType.Chains),
            .. GenerateObjectsDiff(newDifficulty.Chains.Where(x => x.Color == 1).ToList(), oldDifficulty.Chains.Where(x => x.Color == 1).ToList(), CollectionType.Chains),
            .. GenerateObjectsDiff(newDifficulty.Lights, oldDifficulty.Lights, CollectionType.Lights),
            .. GenerateObjectsDiff(newDifficulty.colorBoostBeatmapEvents, oldDifficulty.colorBoostBeatmapEvents, CollectionType.ColorBoostBeatmapEvents),
            .. GenerateObjectsDiff(newDifficulty.lightColorEventBoxGroups, oldDifficulty.lightColorEventBoxGroups, CollectionType.LightColorEventBoxGroups),
            .. GenerateObjectsDiff(newDifficulty.lightRotationEventBoxGroups, oldDifficulty.lightRotationEventBoxGroups, CollectionType.LightRotationEventBoxGroups),
            .. GenerateObjectsDiff(newDifficulty.lightTranslationEventBoxGroups, oldDifficulty.lightTranslationEventBoxGroups, CollectionType.LightTranslationEventBoxGroups),
        ];

        private static List<DiffEntry> GenerateObjectsDiff<T>(List<T> newObjects, List<T> oldObjects, CollectionType collectionType) where T : BeatmapObject
        {
            List<DiffEntry> diffEntries = [];
            ValueRemover<T> oldObjsChecked = new(oldObjects, stackalloc IntPtr[oldObjects.Count]);
            ObjectMatcher<T> oldMatcher = new(oldObjects);

            // checking if new notes have a match in old notes
            foreach(var obj in newObjects)
            {
                (int foundIndex, bool? isInexact) = oldMatcher.CheckMatches(obj);

                if (foundIndex is not -1)
                {
                    // If there are multiple notes on that beat and we matched inexactly, we always find the first one. But we need to check all, so we just loop until we maybe find one on the correct beat
                    while(foundIndex < oldObjsChecked.Items.Length && oldObjsChecked.Items[foundIndex] is null)
                        foundIndex++;

                    if(foundIndex < oldObjsChecked.Items.Length && oldObjsChecked.Items[foundIndex]!.Beats == obj.Beats)
                    {
                        // If yes we mark it as checked
                        oldObjsChecked.Remove(foundIndex);
                    }

                    if (isInexact.HasValue && isInexact.Value)
                    {
                        // handling inexact matches
                        diffEntries.Add(new DiffEntry(DiffType.Modified, obj, collectionType));
                    }
                }
                else
                {
                    // adding new notes
                    diffEntries.Add(new DiffEntry(DiffType.Added, obj, collectionType));
                }
            }

            // adding removed notes
            if(oldObjsChecked.Count > 0)
            {
                foreach(BeatmapObject obj in oldObjsChecked.Enumerate())
                {
                    diffEntries.Add(new DiffEntry(DiffType.Removed, obj, collectionType));
                }
            }

            return diffEntries;
        }

        private readonly ref struct ObjectMatcher<T> where T : BeatmapObject
        {
            readonly Dictionary<BeatmapObject, int> oldObjHashSet;
            readonly Dictionary<float, int>? customEq;

            public ObjectMatcher(List<T> oldObjects)
            {
                oldObjHashSet = new(oldObjects.Count);
                for (int i = 0; i < oldObjects.Count; i++)
                {
                    oldObjHashSet.Add(oldObjects[i], i);
                    if(oldObjects[0] is BpmEvent or BeatmapGridObject)
                    {
                        customEq ??= new(oldObjects.Count);
                        customEq.TryAdd(oldObjects[i].Beats, i);
                    }
                }
            }

            public (int found, bool? isInexact) CheckMatches(BeatmapObject obj)
            {
                if (oldObjHashSet.TryGetValue(obj, out int oldObj))
                {
                    return (oldObj, false);
                }

                if(customEq is not null)
                {
                    return customEq.TryGetValue(obj.Beats, out int oldT) ? (oldT, true) : (-1, null);
                }

                return (-1, null);
            }
        }

        private ref struct ValueRemover<T> where T : class
        {
            public readonly Span<T?> Items;

            public int Count { get; private set; }

            public ValueRemover(List<T> oldList, Span<IntPtr> buffer)
            {
                if(buffer.Length > 0)
                {
                    Items = MemoryMarshal.CreateSpan(ref Unsafe.As<IntPtr, T?>(ref buffer[0]), buffer.Length);
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types. Ok here, because its just a CopyTo ¯\_(ツ)_/¯
                    CollectionsMarshal.AsSpan(oldList).CopyTo(Items);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                }
                Count = Items.Length;
            }

            public void Remove(int index)
            {
                Items[index] = null;
                Count--;
            }

            public readonly ValueRemoveEnumerator Enumerate() => new ValueRemoveEnumerator(this);

            public ref struct ValueRemoveEnumerator(ValueRemover<T> removerP)
            {
                private ValueRemover<T> remover = removerP;
                private int currentIndex;
                public T Current { get; private set; }

                public readonly ValueRemoveEnumerator GetEnumerator() => this;

                public bool MoveNext()
                {
                    do
                    {
#pragma warning disable CS8601 // Possible null reference assignment. Ok here, because its checked afterwards
                        Current = remover.Items[currentIndex++];
#pragma warning restore CS8601 // Possible null reference assignment.
                    } while(currentIndex < remover.Items.Length && Current is null);
                    return currentIndex < remover.Items.Length;
                }

            }
        }

#if NETSTANDARD2_0_OR_GREATER
        public static class CollectionsMarshal
        {
            static class ArrayAccessor<T>
            {
                private static readonly FieldInfo fInfo = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
                public static T[] GetItems(List<T> list) => (T[])fInfo.GetValue(list);
            }
            public static Span<T> AsSpan<T>(List<T> list) => list is null ? default : new Span<T>(ArrayAccessor<T>.GetItems(list), 0, list.Count);
        }
#endif
    }
}
