using System.Collections.Generic;
using System.Linq;
using WindowsEnhancementSuite.Properties;
using WindowsEnhancementSuite.ValueObjects;
using Newtonsoft.Json;

namespace WindowsEnhancementSuite.Helper
{
    public static class RankingHelper
    {
        private static bool initialized;
        private static List<CommandBarRank> rankings;

        public static void Initialize()
        {
            if (initialized) return;
            rankings = JsonConvert.DeserializeObject<List<CommandBarRank>>(Settings.Default.CommandBarRankings) ??
                       new List<CommandBarRank>();
            initialized = true;
        }

        public static void Save()
        {
            if (!initialized) return;
            Settings.Default.CommandBarRankings = JsonConvert.SerializeObject(rankings);
        }

        public static uint GetRank(CommandBarEntry entry)
        {
            if (!initialized) return 0;

            var rank = rankings.FirstOrDefault(e => e.Hash == entry.Hash) ?? new CommandBarRank {Rank = 0};
            return rank.Rank + (uint)entry.Kind;
        }

        public static void IncreaseRank(CommandBarEntry entry)
        {
            if (!initialized) return;

            var rank = rankings.FirstOrDefault(e => e.Hash == entry.Hash);
            if (rank != null)
            {
                uint nextRank = rank.Rank + 1;
                uint nextRankCalls = getFibonacciRecurrence(nextRank);

                rank.Calls++;
                if (rank.Calls >= nextRankCalls) rank.Rank = nextRank;

                return;
            }

            rankings.Add(new CommandBarRank
            {
                Hash = entry.Hash,
                Rank = 0,
                Calls = 1
            });
        }

        private static uint getFibonacciRecurrence(uint value)
        {
            if ((value == 0) || (value == 1)) return value;
            return getFibonacciRecurrence(value - 1) + getFibonacciRecurrence(value - 2);
        }
    }
}
