using System;
using System.Collections.Generic;
using System.Text;

namespace BSMapDiffGenerator.Models
{
    public record class MapDiff(List<DiffEntry> SongInfoDiffEntries, List<DifficultyDiff> DifficultyDiffs);
    public record class DifficultyDiff(string Difficulty, string Characteristic, List<DiffEntry> DiffEntries);
}
