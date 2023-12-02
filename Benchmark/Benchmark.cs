using beatleader_parser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Disassemblers;
using BenchmarkDotNet.Jobs;
using BSMapDiffGenerator;
using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
using Parser.Map;
using Parser.Map.Difficulty.V3.Base;
using Parser.Map.Difficulty.V3.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Benchmark
    {
        BeatmapV3 oldMap;
        BeatmapV3 newMap;

        [GlobalSetup]
        public void Globalsetup()
        {
            oldMap = new Parse().TryDownloadLink(@"https://r2cdn.beatsaver.com/522d0727d30469e09522d193438ec3698b757693.zip")[^1];
            newMap = new Parse().TryDownloadLink(@"https://r2cdn.beatsaver.com/522d0727d30469e09522d193438ec3698b757693.zip")[^1];
            newMap.Difficulties[4].Data.Bombs.Insert(5, new Bomb());
        }

        [Benchmark]
        public void GenerateDifficultyDiff()
        {
            MapDiffGenerator.GenerateDifficultyDiff(newMap.Difficulties[4].Data, oldMap.Difficulties[4].Data);
        }

    }
}
