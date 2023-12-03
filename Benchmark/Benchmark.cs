using beatleader_parser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BSMapDiffGenerator;
using Parser.Map;
using Parser.Map.Difficulty.V3.Grid;

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
