using System;
using System.Collections.Generic;
using System.Text;

namespace BSMapDiffGenerator.Models
{
    public class MapDiff
    {
        public MapDiff(List<DiffEntry> songInfoDiffEntries, List<DifficultyDiff> difficultyDiffs)
        {
            SongInfoDiffEntries = songInfoDiffEntries;
            DifficultyDiffs = difficultyDiffs;
        }

        public List<DiffEntry> SongInfoDiffEntries { get; set; }
        public List<DifficultyDiff> DifficultyDiffs { get; set; }
    }

    public class DifficultyDiff
    {
        public DifficultyDiff(string difficulty, string characteristic, List<DiffEntry> diffEntries)
        {
            Difficulty = difficulty;
            Characteristic = characteristic;
            DiffEntries = diffEntries;
        }

        public string Difficulty { get; set; }
        public string Characteristic { get; set; }
        public List<DiffEntry> DiffEntries { get; set; }
    }
}
